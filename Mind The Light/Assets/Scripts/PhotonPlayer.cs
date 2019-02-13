using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.IO;

public class PhotonPlayer : MonoBehaviour {

   private PhotonView PV;
   public GameObject myChar;

   void Start() {
      PV = GetComponent<PhotonView>();
      int rand = Random.Range(0, GameManager.Instance.spawnPoints.Length);

      if(PV.IsMine) {
         myChar = PhotonNetwork.Instantiate(Path.Combine(Consts.PHOTON_FOLDER, "Guard"), GameManager.Instance.spawnPoints[rand].position, Quaternion.identity, 0);
      }
   }

   void Update() {

   }
}
