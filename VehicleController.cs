﻿using UnityEngine;
using MelonLoader;
using static vehiclemod.data;
namespace vehiclemod
{
	public class VehicleController
	{
		//MISC INTERNAL
		private static Transform[] wheels = new Transform[4];
		private static RaycastHit hit;
		private static Rigidbody rb = null;
		private static AudioSource audio;
		public static Transform myparent;
		//CAMERA PARTS
		public static Transform cameracenter = null;
		public static Transform cameracar = null;
		public static bool fps;
		public static int siter;

		//CAR PARAMS
		public static float maxspeed = 30;
		public static float curspeed = 0;
		public static float curfuel = 50;
		public static float maxfuel = 100;
		public static float accel = 0;
		private static float maxaccel = 30;

		//INPUT MOVE
		private static float curX = 0f;
		private static float curY = 0f;
		public static float move = 0f;
		public static float turn = 0f;

		//WHEEL HIT PARAMS
		private static float wheeldown = 0.4f;
		private static float radius = 0.9f;
		private static float spring = 300f;
		private static float damper = 20f;
		public static void MoveDrive(int number)
		{
			GameObject car = GetObj(number);
			if (!car) return;
			Transform sit = GetObj(number).transform.Find("SIT" + siter);
			NETHost.NETSIT(number, siter);
			CameraFollow(number);//FOLLOW CAMERA
			curX += Input.GetAxis("Mouse X") * 4f;
			curY += Input.GetAxis("Mouse Y") * 4f;
			rb = car.GetComponent<Rigidbody>();
			rb.centerOfMass = Vector3.down * 0.6f;
			curspeed = rb.velocity.magnitude;

			if (!main.allowdrive)
			{
				GameManager.GetVpFPSPlayer().transform.parent = sit;
				GameManager.GetVpFPSPlayer().transform.position = sit.position;
			}

			if (main.allowdrive)
			{
				float dist = Vector3.Distance(GetObj(number).transform.position, GameManager.GetVpFPSPlayer().transform.position);
				if (GetObj(number) && !InterfaceManager.m_Panel_Loading.IsLoading() && dist > 2)
				{
					GetObj(number).transform.position = GameManager.GetVpFPSPlayer().transform.position + GameManager.GetVpFPSPlayer().transform.up * 5f;
					foreach (Collider col in GameManager.GetPlayerTransform().GetComponents<Collider>())
					{
						col.enabled = false;
					}

					GameManager.GetVpFPSPlayer().transform.parent = sit;
					GameManager.GetVpFPSPlayer().transform.position = Vector3.Lerp(GameManager.GetVpFPSPlayer().transform.position, GetObj(number).transform.position, 5);
					if (!cameracar.GetComponent<Camera>())
						cameracar.gameObject.AddComponent<Camera>().CopyFrom(GameManager.GetVpFPSCamera().m_Camera.GetComponent<Camera>());
					cameracar.gameObject.SetActive(true);
				}
				// ACCELERATION
				//MOVING
				if (rb)
				{
					foreach (var i in wheels)
					{

						if (!i) return;
						if (Physics.Raycast(i.transform.position, Vector3.down, out hit, radius + wheeldown))
						{
							Vector3 te = Vector3.Normalize(car.transform.TransformDirection(Vector3.forward));

							if (curspeed > 1) car.transform.Rotate(0, (15f + curspeed / 2) * Time.deltaTime * turn * move, 0);//ROTATE TO
							if (curspeed > maxspeed) return;
							accel = Mathf.Clamp(accel, 0, maxaccel);
							rb.AddForceAtPosition(te * (2f * (10f + accel * 10f)) * move, hit.point); //FORCE DRIVE\\
						}
						Transform wh = i.transform.Find("1");
						Transform wh2 = i.transform.Find("2");
						// 1 - Circle wheel \\
						// 2 - Flat wheel   \\
						if ((wheels[0] == i || wheels[1] == i) && (wh || wh2))
						{
							if (wh && main.allowdrive) wh.localRotation = Quaternion.Euler(0, turn * 30, 0);// VISUAL WHEEL 1 FL
							if (wh && main.allowdrive) wh.localRotation = Quaternion.Euler(0, turn * 30, 0);// VISUAL WHEEL 1 FR
							if (wh2 && main.allowdrive) wh2.localRotation = Quaternion.Euler(0, 0, turn * 30);// VISUAL WHEEL 2 FL
							if (wh2 && main.allowdrive) wh2.localRotation = Quaternion.Euler(0, 0, turn * 30);// VISUAL WHEEL 2 FR
						}
					}
				}
				if (main.vehicledata.ContainsKey(number))
				{
					UpdateDriver(number, true);
					NETHost.NetSendDriver(number, true);
					NETHost.NetSoundOn(number);
					NETHost.NetCar(number, CarData(number)[3], car.transform.position, car.transform.rotation);
				}
			}
			NETHost.NETSIT(number, siter);
			EngineSound(number, 1);
		}
		public static void PlayerCarMove(int CarID, string CarName, Vector3 Position, Quaternion Rotation)
		{
			if (!GetObj(CarID) || !main.vehicles.ContainsKey(CarID))
			{
				main.SpawnCar(CarID, main.MyId, CarName, Position, Rotation);
			} else {
				GetObj(CarID).transform.rotation = Rotation;
				if (Vector3.Distance(GetObj(CarID).transform.position, Position) > 10)
					GetObj(CarID).transform.position = Position;
				else
					GetObj(CarID).transform.position = Vector3.Lerp(GetObj(CarID).transform.position, Position, 15);
			}
		}
		public static void wheel(int number)
		{
			GameObject car = GetObj(number);
			if (!car) return;

			Transform[] wheels = new Transform[4];// Hori Vertic
			wheels[0] = car.transform.Find("FL"); // Left Foward
			wheels[1] = car.transform.Find("FR"); // Right Forward
			wheels[2] = car.transform.Find("BL"); // Backward Left
			wheels[3] = car.transform.Find("BR"); // Backward Right

			Rigidbody rb = car.GetComponent<Rigidbody>();

			if (rb)
				foreach (var i in wheels)
				{
					if (!i) return;
					Transform wh = i.transform.Find("1");
					Transform wh2 = i.transform.Find("2");
					if (wh || wh2)
					{
						// 1 - Circle wheel \\
						// 2 - Flat wheel   \\
						if (wh && Physics.Raycast(i.transform.position, Vector3.down, out hit, 0.6f))
						{
							wh.transform.position = hit.point + Vector3.up * 0.5f;
						}
						if (wh2 && Physics.Raycast(i.transform.position, Vector3.down, out hit, 0.1f))
						{
							wh2.transform.position = hit.point + Vector3.up * 0.05f;
						}
						//SETTING THE ACCEL AND OTHER FOR
						if (wh) { maxaccel = 20f; wheeldown = 0.4f; radius = 0.9f; spring = 300f; damper = 20f; } // Setting for WHEEL CIRCLE
						if (wh2) { maxaccel = 1f; wheeldown = 0.1f; radius = 0.5f; spring = 300f; damper = 20f; } // Setting for WHEEL flat

						if ((wh || wh2) && Physics.Raycast(i.transform.position, Vector3.down, out hit, radius + wheeldown))
						{
							Vector3 velocityAtTouch = rb.GetPointVelocity(hit.point);
							float compression = -(hit.distance / radius + wheeldown) + 1;
							Vector3 force = Vector3.up * compression * spring;
							Vector3 t = car.transform.InverseTransformDirection(velocityAtTouch);
							Vector3 damping = car.transform.TransformDirection(t) * -damper;
							Vector3 finalForce = force + damping;
							t = car.transform.TransformDirection(t);

							rb.AddForceAtPosition(finalForce + (t), hit.point);
						}
					}
				}
		}
		public static void SitCar(int number)
		{
			GameObject car = GetObj(number);
			siter = CountPassangers(number) + 1;

			if (main.isSit) siter = 0;

			UpdatePassanger(number, siter, main.MyId);
			NETHost.NETSIT(number, siter);

			if (main.isSit)
			{
				if (main.allowdrive)
				{
					data.UpdateDriver(number, false);
					NETHost.NetSendDriver(number, false);
				}

				GameManager.GetPlayerManagerComponent().TeleportPlayer(main.MyPosition.position - car.transform.right * 2.5f + car.transform.up, Quaternion.identity);
				GameManager.GetPlayerManagerComponent().StickPlayerToGround();
				foreach (Collider col in GameManager.GetPlayerTransform().GetComponents<Collider>())
				{
					col.enabled = true;
				}
				main.allowdrive = false;
				cameracar.gameObject.SetActive(false);
				cameracar.SetParent(null);
				main.targetcar = -1;
				MenuControll.Open(1);
				if (CountPassangers(number) <= 1)
				{
					EngineSound(number, 0);
					NETHost.NetSoundOff(number);
				}
				main.isSit = false;
				GameManager.GetVpFPSPlayer().transform.SetParent(VehicleController.myparent);
				NETHost.NETSIT(number, 0);
			}
			else
			{
				Transform sit = car.transform.Find("SIT" + siter);
				if (!sit) return;
				if (main.allowdrive)
				{
					data.UpdateDriver(number, true);
					NETHost.NetSendDriver(number, true);
				}
				foreach (Collider col in GameManager.GetPlayerTransform().GetComponents<Collider>())
				{
					col.enabled = false;
				}
				wheels[0] = car.transform.Find("FR");
				wheels[1] = car.transform.Find("FL");
				wheels[2] = car.transform.Find("BL");
				wheels[3] = car.transform.Find("BR");
				cameracar.gameObject.SetActive(true);
				MenuControll.Open(1);
				cameracenter = car.transform.Find("CAMERACENTER");
				cameracar.transform.position = cameracenter.position;
				main.isSit = true;
				GameManager.GetVpFPSPlayer().transform.SetParent(sit);
				GameManager.GetVpFPSPlayer().transform.position = sit.position;
			}

			MelonLogger.Msg("=================================");
			MelonLogger.Msg("[Sit Manager]: ID | LOCALL | SERVER | COUNTPASSANGER");
			MelonLogger.Msg("[Sit Manager]: " + number + " | " + main.allowdrive + " | " + isDrive(number) + " | " + CountPassangers(number)+ " | " + siter);
			MelonLogger.Msg("=================================");

		}
		public static void EngineSound(int ID, int state)
		{
			if (!GetObj(ID)) return;
			if (curspeed == 0) curspeed = 1;

			audio = GetObj(ID).GetComponent<AudioSource>();

			if (state == 0)
			{
				audio.Stop();
				return;
			}

			if (!audio.isPlaying) audio.Play();
			if (audio)
			{
				var dist = Vector3.Distance(GetObj(ID).transform.position, main.MyPosition.position);

				audio.volume = calk(dist);

				audio.pitch = (0.30f + curspeed * 0.025f);
				if (curspeed > 30)
				{
					audio.pitch = (0.25f + curspeed * 0.015f);
				}
				if (curspeed > 40)
				{
					audio.pitch = (0.20f + curspeed * 0.013f);
				}
				if (curspeed > 49)
				{
					audio.pitch = (0.15f + curspeed * 0.011f);
				}

				if (audio.pitch > 2.0f)
				{
					audio.pitch = 2.0f;
				}
			}
		}
		private static void CameraFollow(int CarId)
		{
			main.vehicles.TryGetValue(CarId, out GameObject car);
			if (!fps)
			{
				GameObject player = GetObj(CarId).transform.Find("SIT" + siter + "/" + main.MyId).gameObject;
				if (player && !player.active) player.SetActive(true);

				if (main.allowdrive)
					cameracar.transform.SetParent(null);
				else
					cameracar.transform.SetParent(car.transform);
				Vector3 range = -Vector3.forward * 6f;
				float smothspeed = 5f;
				cameracenter.transform.localRotation = Quaternion.Euler(-curY, -curX, 0);//ROTATE CENTER

				Vector3 targetPosition = cameracenter.position + cameracenter.forward * range.z + cameracenter.right * range.x + cameracenter.up * range.y;
				cameracar.transform.position = Vector3.Lerp(cameracar.transform.position, targetPosition, smothspeed * Time.deltaTime);
				cameracar.transform.LookAt(cameracenter);
				GameManager.GetVpFPSCamera().transform.LookAt(cameracar);
			}
			else
			{
				GameObject player = GetObj(CarId).transform.Find("SIT" + siter + "/" + main.MyId).gameObject;
				if (player && player.active) player.SetActive(false);
				cameracar.transform.SetParent(car.transform);
				curY = Mathf.Clamp(curY, -60, 90);
				curX = Mathf.Clamp(curX, -180, 180);
				cameracar.transform.localRotation = Quaternion.Euler(-curY, curX, 0);
				if (car.transform.Find("SIT" + siter)) cameracar.transform.position = car.transform.Find("SIT" + siter).position + car.transform.up * 1.7f;
			}
		}
		private static float calk(float i)
		{
			i = i / 100;
			i = 1 - i;
			Mathf.Clamp(i, 0, 1);
			return i;
		}
		public static void CarLight(int CarId, bool state)
		{
			GameObject obj = GetObj(CarId);
			if (!obj) return;

			GameObject LeftTorch = obj.transform.Find("LF").gameObject;
			GameObject RightTorch = obj.transform.Find("RF").gameObject;
			if (LeftTorch)
			{
				LeftTorch.GetComponent<Light>().enabled = state;
			}
			if (RightTorch)
			{
				RightTorch.GetComponent<Light>().enabled = state;
			}
			main.light = state;
		}
	}
}
