using UnityEngine;
using MelonLoader;
using static vehiclemod.data;
using static vehiclemod.NETHost;
namespace vehiclemod
{
	public class VehicleController
	{
		//MISC INTERNAL
		private static Transform[] wheels = new Transform[4];
		private static RaycastHit hit;
		private static Rigidbody rb = null;
		private static AudioSource audio;

		//CAMERA PARTS
		public static Transform cameracenter = null;
		public static Transform cameracar = null;
		public static bool fps;
		//private static AudioSource audio;

		//CAR PARAMS
		public static float maxspeed = 40;
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
		public static void MoveDrive(GameObject car)
		{
			if (!car) return;
			if (!main.vehicles.ContainsKey(int.Parse(car.name))) return;
			Transform sit = car.transform.Find("SIT1");
			CameraFollow(car);//FOLLOW CAMERA
			curspeed = rb.velocity.magnitude;
			curX += Input.GetAxis("Mouse X") * 4f;
			curY += Input.GetAxis("Mouse Y") * 4f;
			rb = car.GetComponent<Rigidbody>();
			rb.centerOfMass = Vector3.down * 0.9f;

			if (move != 0 && main.allowdrive) //ACELERATION
				accel += 5 * Time.deltaTime;
			else
				accel = 1;

			accel = Mathf.Clamp(accel, 0, maxaccel);
			if (main.allowdrive)
			{
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
							if (move == 0) rb.AddForceAtPosition(te * curspeed * (15f + accel), hit.point);//SAVING SPEED for slide

							if (curspeed > 1) car.transform.Rotate(0, (15f + curspeed / 2) * Time.deltaTime * turn * move, 0);//ROTATE TO
							if (curspeed > maxspeed) return;
							rb.AddForceAtPosition(te * (2f * (10f + accel * 10f)) * move, hit.point); //FORCE DRIVE\\
						}
						Transform wh = i.transform.Find("1");
						Transform wh2 = i.transform.Find("2");
						// 1 - Circle wheel \\
						// 2 - Flat wheel   \\
						if ((wheels[0] == i || wheels[1] == i) && (wh || wh2))
						{
							if (wh && main.allowdrive) wh.localRotation = Quaternion.Euler(0, turn * 40, 0);// VISUAL WHEEL 1 FL
							if (wh && main.allowdrive) wh.localRotation = Quaternion.Euler(0, turn * 40, 0);// VISUAL WHEEL 1 FR
							if (wh2 && main.allowdrive) wh2.localRotation = Quaternion.Euler(0, 0, turn * 30);// VISUAL WHEEL 2 FL
							if (wh2 && main.allowdrive) wh2.localRotation = Quaternion.Euler(0, 0, turn * 30);// VISUAL WHEEL 2 FR
						}
					}
				}
				if (main.vehicledata.ContainsKey(int.Parse(car.name)))
				{
					UpdateDriver(int.Parse(car.name), true);
					NetSendDriver(int.Parse(car.name), true);
					NetSoundOn((int)Mathf.Round(curspeed), int.Parse(car.name));
					NetCar(int.Parse(car.name),CarData(int.Parse(car.name))[3], car.transform.position, car.transform.rotation);
				}
			}
			engineSound(curspeed, int.Parse(car.name));
		}
		public static void PlayerCarMove(int CarID, string CarName, Vector3 Position, Quaternion Rotation)
		{
			if (!main.vehicles.ContainsKey(CarID)) return;

			GetObj(CarID).transform.rotation = Rotation;
			GetObj(CarID).transform.position = Vector3.Lerp(GetObj(CarID).transform.position, Position, 15);
		}
		public static void wheel(GameObject car)
		{
			if (!car) return;

			Transform[] wheels = new Transform[4];// Hori Vertic
			wheels[0] = car.transform.Find("FL"); // Left Foward
			wheels[1] = car.transform.Find("FR"); // Right Forward
			wheels[2] = car.transform.Find("BL"); // Backward Left
			wheels[3] = car.transform.Find("BR"); // Backward Right

			Rigidbody rb = car.GetComponent<Rigidbody>();

			if (rb) foreach (var i in wheels)
				{
					if (!i) return;
					Transform wh = i.transform.Find("1");
					Transform wh2 = i.transform.Find("2");
					if (wh || wh2)
					{
						// 1 - Circle wheel \\
						// 2 - Flat wheel   \\
						if (wheels[0] == i || wheels[1] == i)
						{
							if (wh && Physics.Raycast(i.transform.position, Vector3.down, out hit, 0.6f))
							{
								wh.transform.position = hit.point + Vector3.up * 0.5f;
							}
							if (wh2 && Physics.Raycast(i.transform.position, Vector3.down, out hit, 0.1f))
							{
								wh2.transform.position = hit.point + Vector3.up * 0.05f;
							}
						}
						//SETTING THE ACCEL AND OTHER FOR
						if (wh) { maxaccel = 20f; wheeldown = 0.4f; radius = 0.9f; spring = 300f; damper = 20f; } // Setting for WHEEL CIRCLE
						if (wh2) { maxaccel = 1f; wheeldown = 0.1f; radius = 0.5f; spring = 300f; damper = 20f; } // Setting for WHEEL flat

						if ((wh || wh2) && Physics.Raycast(i.transform.position, Vector3.down, out hit, radius + wheeldown))
						{
							Vector3 velocityAtTouch = rb.GetPointVelocity(hit.point);
							float compression = -(hit.distance / radius + wheeldown) + 1;
							Vector3 force = Vector3.up * compression * spring;
							Vector3 t = i.transform.InverseTransformDirection(velocityAtTouch);
							Vector3 damping = i.transform.TransformDirection(t) * -damper;
							Vector3 finalForce = force + damping;
							t.z = 0;
							t.x = 0;
							rb.AddForceAtPosition(finalForce + (t), hit.point);
						}
					}
				}
		}
		public static void SitCar(GameObject car)
		{
			if (!car) return;
			MelonLogger.Msg("[Interact] Trying to interact with car ID:> " + car.name);
			audio = car.GetComponent<AudioSource>();
			rb = car.GetComponent<Rigidbody>();
			bool ise;
			if (main.isSit)
			{
				if (main.allowdrive)
				{
					data.UpdateDriver(int.Parse(car.name), false);
					NetSendDriver(int.Parse(car.name), false);
				}
				main.isSit = false;
				main.allowdrive = false;
				main.targetcar = null;

				foreach (Collider col in GameManager.GetVpFPSPlayer().GetComponents<Collider>())
				{
					col.enabled = true;
				}
				cameracar.gameObject.SetActive(false);
				GameManager.GetVpFPSPlayer().transform.parent = null;
				GameManager.GetPlayerTransform().transform.parent = null;
				GameManager.GetPlayerManagerComponent().TeleportPlayer(main.MyPosition.position - car.transform.right * 2.5f + car.transform.up, Quaternion.identity);
				GameManager.GetPlayerManagerComponent().StickPlayerToGround();
				MenuControll.Open(1);
				if(audio) audio.Stop();
				NetSoundOff(int.Parse(car.name));
			}
			else
			{
				if (main.allowdrive)
				{
					data.UpdateDriver(int.Parse(car.name), true);
					NetSendDriver(int.Parse(car.name), true);
				}
				foreach (Collider col in GameManager.GetVpFPSPlayer().GetComponents<Collider>())
				{
					col.enabled = false;
				}
				wheels[0] = car.transform.Find("FR");
				wheels[1] = car.transform.Find("FL");
				wheels[2] = car.transform.Find("BL");
				wheels[3] = car.transform.Find("BR");
				cameracar.gameObject.SetActive(true);
				main.isSit = true;
				MenuControll.Open(1);
				cameracenter = car.transform.Find("CAMERACENTER");
				cameracar.transform.position = car.transform.Find("CAMERACENTER").position;
				main.targetcar = car;
				Transform sit = car.transform.Find("SIT1");
				GameManager.GetVpFPSPlayer().transform.position = sit.position;
				GameManager.GetPlayerTransform().transform.position = sit.position;
				GameManager.GetVpFPSPlayer().transform.parent = car.transform;
				GameManager.GetPlayerTransform().transform.parent = car.transform;
			}
			MelonLogger.Msg("[Sit Manager]: " + main.allowdrive + "|" +isDrive(int.Parse(car.name)));
		}
		public static void engineSound(float curspeed, int ID)
		{
			if (!data.GetObj(ID)) return;
			audio =GetObj(ID).GetComponent<AudioSource>();
			if(!audio.isPlaying) audio.Play();

			if (!audio.isPlaying && audio)
			{
				audio.maxDistance = 30f;
				audio.minDistance = 0f;
				var dist = Vector3.Distance(main.MyPosition.position,GetObj(ID).transform.position); ;
				if (dist > 10) audio.volume = dist * 0.05f;
				if (dist > 20) audio.volume = dist * 0.05f * 0.5f;
				if (dist < 1) audio.volume = dist;
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
		public static void engineSoundOff(int ID)
		{
			if (!data.GetObj(ID)) return;
			audio =GetObj(ID).GetComponent<AudioSource>();
			if(audio) audio.Stop();
		}
			private static void CameraFollow(GameObject car)
		{
			if (!fps)
			{
				cameracar.transform.parent = null;
				Vector3 range = -Vector3.forward * 6f;
				float smothspeed = 5f;
				cameracenter.transform.localRotation = Quaternion.Euler(-curY, -curX, 0);//ROTATE CENTER

				Vector3 targetPosition = cameracenter.position + cameracenter.forward * range.z + cameracenter.right * range.x + cameracenter.up * range.y;
				cameracar.transform.position = Vector3.Lerp(cameracar.transform.position, targetPosition, smothspeed * Time.deltaTime);
				cameracar.transform.LookAt(cameracenter);
			}
			else
			{
				cameracar.transform.parent = car.transform;
				cameracar.transform.localRotation = Quaternion.Euler(-curY, curX, 0);
				cameracar.transform.position = car.transform.Find("SIT1").position + car.transform.up * 1.75f;
			}
		}
	}
}
