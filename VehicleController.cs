using UnityEngine;
using MelonLoader;
using static vehiclemod.data;

namespace vehiclemod
{
	public class VehicleController
	{
		//MISC INTERNAL
		public static Transform myparent;
		//CAMERA PARTS
		public static Transform cameracenter = null;
		public static Transform cameracar = null;
		public static bool fps;
		public static int siter;
		//INPUT MOVE
		private static float curX = 0f;
		private static float curY = 0f;
		public static Transform sit;
		public static void MoveDrive(int number)
		{
			GameObject car = GetObj(number);
			if (!car) return;
			CameraFollow(number);//FOLLOW CAMERA
			curX += Input.GetAxis("Mouse X") * 4f;
			curY += Input.GetAxis("Mouse Y") * 4f;
			if (car)
				if (main.allowdrive)
				{
					float dist = Vector3.Distance(car.transform.position, GameManager.GetVpFPSPlayer().transform.position);
					if (!InterfaceManager.m_Panel_Loading.IsLoading() && dist > 2)
					{
						car.transform.position = GameManager.GetVpFPSPlayer().transform.position + GameManager.GetVpFPSPlayer().transform.up * 5f;
						foreach (Collider col in GameManager.GetPlayerTransform().GetComponents<Collider>()) col.enabled = false;

						GameManager.GetVpFPSPlayer().transform.parent = sit;
						GameManager.GetVpFPSPlayer().transform.position = Vector3.Lerp(GameManager.GetVpFPSPlayer().transform.position, car.transform.position, 5);
						cameracar.gameObject.SetActive(true);
					}
				}
				else
				{
					cameracar.gameObject.SetActive(true);
					GameManager.GetVpFPSPlayer().transform.parent = sit;
					GameManager.GetVpFPSPlayer().transform.position = Vector3.Lerp(GameManager.GetVpFPSPlayer().transform.position, car.transform.position, 5);
				}
		}
		public static void SitCar(int number)
		{
			main.CHECKCAMERA();
			GameObject car = GetObj(number);
			InfoMain data = car.GetComponent<VehComponent>().vehicleData;
			siter = CountPassangers(number);
			if (main.allowdrive) siter = 0;
			if (main.isSit) siter = -1;
			// PARENT THE LOCAL 3D MODEL TO VEHICLE OR unparent it
			if (main.isSit)
			{
				main.targetcar = -1;
				if (main.allowdrive)
				{
					UpdateDriver(number, false);
					NETHost.NetSendDriver(number, false);
				}
				GameManager.GetPlayerManagerComponent().TeleportPlayer(main.MyPosition.position - car.transform.right * 2.5f + car.transform.up, Quaternion.identity);
				GameManager.GetPlayerManagerComponent().StickPlayerToGround();
				foreach (Collider col in GameManager.GetPlayerTransform().GetComponents<Collider>()) col.enabled = true;
				main.allowdrive = false;
				cameracar.gameObject.SetActive(false);
				cameracar.SetParent(null);
				main.targetcar = -1;
				MenuControll.Open(1);
				if (!isDrive(number)) NETHost.NetSound(number, false);
				main.isSit = false;
				GameManager.GetVpFPSPlayer().transform.SetParent(myparent);
				sit = null;
			}
			else
			{
				if(data.m_Sits.Count >= siter) sit = data.m_Sits[siter];
				if (!sit) { main.targetcar = -1; return; }
				Transform parenter = GetObj(number).transform.Find("SITS");
				if (main.allowdrive)
				{
					UpdateDriver(number, true);
					NETHost.NetSendDriver(number, true);
				}
				foreach (Collider col in GameManager.GetPlayerTransform().GetComponents<Collider>()) col.enabled = false;
				cameracar.gameObject.SetActive(true);
				cameracenter = car.transform.Find("CAMERACENTER");
				cameracar.transform.position = cameracenter.position;
				main.isSit = true;
				MenuControll.Open(11);
				GameManager.GetVpFPSPlayer().transform.SetParent(parenter);
				GameManager.GetVpFPSPlayer().transform.position = sit.position;
				NETHost.NetSound(number, true);
				if (!GetObj(number).GetComponent<VehComponent>().vehicleData.m_SoundPlay) UpdateSound(number, true);
			}

			UpdatePassanger(number, siter, main.MyId);
			NETHost.NETSIT(number, siter);
			MelonLogger.Msg("[Sit Manager]: " + number + " | " + main.allowdrive + " | " + isDrive(number) + " | " + CountPassangers(number) + " | " + siter);
		}
		private static void CameraFollow(int CarId)
		{
			GameObject car = GetObj(CarId);
			if (!fps)
			{
				GameObject player = GetLocalSit(CarId, siter).gameObject;
				cameracar.transform.SetParent(null);
				if (player && !player.active) player.SetActive(true);
				Vector3 range = -Vector3.forward * 6f;
				float smothspeed = 5f;
				cameracenter.transform.localRotation = Quaternion.Euler(-curY, -curX, 0);//ROTATE CENTER

				Vector3 targetPosition = cameracenter.position + cameracenter.forward * range.z + Vector3.right * range.x + Vector3.up * range.y;
				cameracar.transform.position = Vector3.Lerp(cameracar.transform.position, targetPosition, smothspeed * Time.deltaTime);
				cameracar.transform.LookAt(cameracenter);
			}
			else
			{
				GameObject player = GetLocalSit(CarId, siter).gameObject;
				if (player && player.active) player.SetActive(false);
				cameracar.transform.SetParent(car.transform);
				curY = Mathf.Clamp(curY, -60, 90);
				curX = Mathf.Clamp(curX, -180, 180);
				cameracar.transform.localRotation = Quaternion.Euler(-curY, curX, 0);
				cameracar.transform.position = sit.position + sit.transform.up * 1.7f;
			}
		}
		private static Transform GetLocalSit(int CarId, int place)
		{
			InfoMain a = GetObj(CarId).GetComponent<VehComponent>().vehicleData;
			if (place == -1) return null;
			if (!a.m_Sits[place]) return null;
			return a.m_Sits[place].GetChild(a.m_Sits[place].GetChildCount() - 1);
		}
	}
}