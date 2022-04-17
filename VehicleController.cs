using UnityEngine;
using MelonLoader;
using static vehiclemod.data;
using BringBackComponents;

namespace vehiclemod
{
	public class VehicleController
	{
		//MISC INTERNAL
		private static Transform[] wheels = new Transform[4];
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
		public static float prevspeed = 0;
		public static float curfuel = 50;
		public static float maxfuel = 100;
		//INPUT MOVE
		private static float curX = 0f;
		private static float curY = 0f;
		public static float move = 0f;
		public static float turn = 0f;
		public static float MotorTorque = 20f;
		public static void MoveDrive(int number)
		{

			GameObject car = GetObj(number);
			if (!car) return;
			NETHost.NETSIT(number, siter);
			CameraFollow(number);//FOLLOW CAMERA
			curX += Input.GetAxis("Mouse X") * 4f;
			curY += Input.GetAxis("Mouse Y") * 4f;
			curspeed = car.GetComponent<Rigidbody>().velocity.magnitude;

			if (main.allowdrive && !main.isChat)
			{
				float dist = Vector3.Distance(GetObj(number).transform.position, GameManager.GetVpFPSPlayer().transform.position);
				if (GetObj(number) && !InterfaceManager.m_Panel_Loading.IsLoading() && dist > 2)
				{
					Transform sit = GetObj(number).transform.Find("SIT" + siter);
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

				MotorTorque = 1000 * Time.fixedDeltaTime;
				if (curspeed <= 0.05) MotorTorque = 1000 * Time.fixedDeltaTime * curspeed;

				for (int i = 0; i != wheels.Length; i++)
				{
					if (move != 0 && !Input.GetKey(KeyCode.Space))
					{
						WheelComponent.Set_BrakeTorque(wheels[i], 0);
						WheelComponent.Set_MotorTorque(wheels[i], MotorTorque * move);
					} else {
						WheelComponent.Set_MotorTorque(wheels[i], 0);
						WheelComponent.Set_BrakeTorque(wheels[i], MotorTorque * 3);
					}
				}
				
				WheelComponent.Set_SteerAngle(wheels[0], Mathf.Clamp(curspeed * turn + turn * 10, -45, 45));
				WheelComponent.Set_SteerAngle(wheels[1], Mathf.Clamp(curspeed * turn + turn * 10, -45, 45));

				UpdateDriver(number, true);
				NETHost.NetSendDriver(number, true);
				NETHost.NetSoundOn(number);
				NETHost.NetCar(number, car.transform.position, car.transform.rotation);
			}
			if (!main.allowdrive)
			{
				Transform sit = GetObj(number).transform.Find("SIT" + siter);
				GameManager.GetVpFPSPlayer().transform.parent = sit;
				GameManager.GetVpFPSPlayer().transform.position = sit.position;
			}
			NETHost.NETSIT(number, siter);
			EngineSound(number, 1);
		}
		public static void wheel(GameObject car)
		{
			Transform[] wheels = new Transform[4];
			wheels[0] = car.transform.Find("FR");
			wheels[1] = car.transform.Find("FL");
			wheels[2] = car.transform.Find("BL");
			wheels[3] = car.transform.Find("BR");

			foreach (var wheelpost in wheels)
			{
				Transform Wheel_BodyTarget = wheelpost.GetChild(0);
				if (!Wheel_BodyTarget) return;
				WheelComponent.Get_WorldPose(wheelpost, out Vector3 pos, out Quaternion rot);
				Wheel_BodyTarget.position = pos;
				Wheel_BodyTarget.rotation = rot;
			}
		}
		public static void PlayerCarMove(int CarID, Vector3 Position, Quaternion Rotation)
		{
			Transform car = GetObj(CarID).transform;
			if (!car || !main.vehicles.ContainsKey(CarID)) 
				main.SpawnCar(CarID, main.levelid, CarData(CarID)[3], Position, Rotation);
			else
			{
				car.transform.rotation = Quaternion.Lerp(car.transform.rotation, Rotation, 2f);
				if (Vector3.Distance(car.transform.position, Position) > 10)
					car.transform.position = Position;
				else
					car.transform.position = Vector3.Lerp(car.transform.position, Position, 15);
			}
		}
		public static void SitCar(int number)
		{
			GameObject car = GetObj(number);
			siter = CountPassangers(number) + 1;
			if (main.allowdrive) siter = 1;
			if (main.isSit) siter = 0;

			UpdatePassanger(number, siter, main.MyId);
			NETHost.NETSIT(number, siter);
			// PARENT THE LOCAL 3D MODEL TO VEHICLE OR unparent it
			if (main.isSit)
			{
				if (main.allowdrive)
				{
					data.UpdateDriver(number, false);
					NETHost.NetSendDriver(number, false);
					foreach(var i in wheels)
                    {
						WheelComponent.Set_MotorTorque(i, 0);
						WheelComponent.Set_BrakeTorque(i, 2000);
					}
				}

				GameManager.GetPlayerManagerComponent().TeleportPlayer(main.MyPosition.position - car.transform.right * 2.5f + car.transform.up, Quaternion.identity);
				GameManager.GetPlayerManagerComponent().StickPlayerToGround();
				foreach (Collider col in GameManager.GetPlayerTransform().GetComponents<Collider>()) col.enabled = true;
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
				GameManager.GetVpFPSPlayer().transform.SetParent(myparent);
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
				foreach (Collider col in GameManager.GetPlayerTransform().GetComponents<Collider>()) col.enabled = false;

				wheels[0] = car.transform.Find("FR");
				wheels[1] = car.transform.Find("FL");
				wheels[2] = car.transform.Find("BL");
				wheels[3] = car.transform.Find("BR");

				cameracar.gameObject.SetActive(true);
				cameracenter = car.transform.Find("CAMERACENTER");
				cameracar.transform.position = cameracenter.position;
				main.isSit = true;
				MenuControll.Open(11);
				GameManager.GetVpFPSPlayer().transform.SetParent(sit);
				GameManager.GetVpFPSPlayer().transform.position = sit.position;
			}

			MelonLogger.Msg("=================================");
			MelonLogger.Msg("[Sit Manager]: ID | LOCALL | SERVER | COUNTPASSANGER");
			MelonLogger.Msg("[Sit Manager]: " + number + " | " + main.allowdrive + " | " + isDrive(number) + " | " + CountPassangers(number) + " | " + siter);
			MelonLogger.Msg("=================================");

		}
		public static void EngineSound(int ID, int state) // SOUNDS FOR VEHICLE
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
				audio.volume = calkrange(dist);
				audio.pitch = (0.30f + curspeed * 0.025f);
				if (curspeed > 30) audio.pitch = (0.25f + curspeed * 0.015f);
				if (curspeed > 40) audio.pitch = (0.20f + curspeed * 0.013f);
				if (curspeed > 49) audio.pitch = (0.15f + curspeed * 0.011f);
				if (audio.pitch > 2.0f) audio.pitch = 2.0f;

			}
		}
		private static void CameraFollow(int CarId)
		{
			// CAMERA CONTROLLER FOR VEHICLE
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
		public static void CarLight(int CarId, bool state) // Spot light on car
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