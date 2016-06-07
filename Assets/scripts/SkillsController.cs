﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.scripts
{
	public class SkillsController : MonoBehaviour
	{
		public static GameObject TargetBrackets;
		public static bool IsActive;
		public static List<Coordinate> TargetCoordinates;
		public static List<GameObject> Brackets; 
		public static int MaxTargets;
		public static GameObject Hero;

		public static SkillsController Instance;
		// Use this for initialization
		void Start ()
		{
			IsActive = false;
			MaxTargets = 0;
			TargetCoordinates = new List<Coordinate>();
			Brackets = new List<GameObject>();
			Instance = this;
		}

		public static void Activate(int numOfTargets)
		{
			IsActive = true;
			MaxTargets = numOfTargets;
			TargetCoordinates = new List<Coordinate>();
			Brackets = new List<GameObject>();

		}
		public IEnumerator ThrowFireball(int size)
		{
			AudioHolder.PlayFireBall();
			Game.PlayerIsBlocked = true;
			var fireball = GameObject.Find("DeathPrefab(Clone)").transform.FindChild("FireBall").gameObject;
			fireball.SetActive(true);
			fireball.GetComponent<Animation>().Play();
			var distance = GameField.GetVectorFromCoord(
				TargetCoordinates.ElementAt(0).X, TargetCoordinates.ElementAt(0).Y) - GameObject.Find("DeathPrefab(Clone)").transform.position;
			var posCopy = fireball.transform.position;
			var scaleCopy = fireball.transform.localScale;
			for (int i = 0; i < 50; i++)
			{
				posCopy.x += distance.x / 50;
				posCopy.y += distance.y / 50;
				scaleCopy.x += 0.01f/(3f/size);
				scaleCopy.y += 0.01f/(3f/size);
				fireball.transform.position = posCopy;
				fireball.transform.localScale = scaleCopy;
				yield return new WaitForSeconds(0.01f);
			}
			fireball.transform.position = new Vector3(-3.47f, 1.84f);
			fireball.transform.localScale = new Vector3(0.1f, 0.1f, 1f);
			fireball.SetActive(false);
			Game.PlayerIsBlocked = false;
			if (size == 3)
				GameField.BurnSquare3X3(TargetCoordinates.ElementAt(0));
			if (size == 2)
				GameField.BurnCross(TargetCoordinates.ElementAt(0));
			if (size == 1)
				GameField.BurnCell(TargetCoordinates.ElementAt(0));
			AudioHolder.PlayMassRemove();
		}
		public static void Deactivate()
		{
			MaxTargets = 0;
			IsActive = false;
			foreach(var bracket in Brackets)
				Destroy(bracket);
			Brackets = new List<GameObject>();
		}
		// Update is called once per frame
		void Update ()
		{
			if (!IsActive) return;
			if (MaxTargets == 0 || TargetCoordinates.Count>0)
				ActivateYes();
			else
				DeactivateYes();
		}

		void DeactivateYes()
		{
			
			var btn = GetComponentInChildren<Button>();
			btn.interactable = false;
			btn.GetComponent<Image>().color = Color.gray;
		}

		void ActivateYes()
		{
			var btn = GetComponentInChildren<Button>();
			btn.interactable = true;
			btn.GetComponent<Image>().color = Color.white;
		}

		public static void BracketMonster(Coordinate gridPosition)
		{
			Debug.Log(MaxTargets);
			if (Brackets.Any(x => x.transform.position == GameField.GetVectorFromCoord(gridPosition.X, gridPosition.Y)))
			{
				TargetCoordinates.Remove(gridPosition);
				var bracketToRemove = Brackets.First(x => x.transform.position == GameField.GetVectorFromCoord(gridPosition.X, gridPosition.Y));
				Brackets.Remove(bracketToRemove);
				Destroy(bracketToRemove.gameObject);

				return;
			}
			if (MaxTargets == TargetCoordinates.Count) return;
			var bracket = (GameObject) Instantiate(TargetBrackets,
				GameField.GetVectorFromCoord(gridPosition.X, gridPosition.Y), Quaternion.Euler(new Vector3()));
			Brackets.Add(bracket);
			TargetCoordinates.Add(gridPosition);
		}

		public IEnumerator ThrowIceBall(int skill3Level)
		{
			Game.PlayerIsBlocked = true;
			var iceBallPrefab = Resources.Load("objects/heroes/DeathHero/Ice/IceBall", typeof(GameObject)) as GameObject;

			for (int i = 0; i < TargetCoordinates.Count;i++)
			{
				var distance = GameField.GetVectorFromCoord(
					TargetCoordinates.ElementAt(i).X, TargetCoordinates.ElementAt(0).Y) - GameObject.Find("DeathPrefab(Clone)").transform.position;
				var iceBall =
					(GameObject)
						Instantiate(iceBallPrefab, GameObject.Find("DeathPrefab(Clone)").transform.position,
							Quaternion.Euler(new Vector3()));
				var posCopy = iceBall.transform.position;
				var scaleCopy = iceBall.transform.localScale;
				for (int j = 0; j < 20; j++)
				{
					posCopy.x += distance.x / 20;
					posCopy.y += distance.y / 20;
					scaleCopy.x += 0.05f;
					scaleCopy.y += 0.05f;
					iceBall.transform.position = posCopy;
					iceBall.transform.localScale = scaleCopy;
					iceBall.transform.Rotate(new Vector3(0,0,0.1f));
					yield return new WaitForSeconds(0.01f);
				}
				if (GameField.Map[TargetCoordinates.ElementAt(i).X, TargetCoordinates.ElementAt(i).Y].TypeOfMonster ==
				    MonsterType.BlackHole)
				{
					GameField.Map[TargetCoordinates.ElementAt(i).X, TargetCoordinates.ElementAt(i).Y].State
						= MonsterState.Deactivated;
					var iceCellPrefab = Resources.Load("objects/heroes/DeathHero/Ice/IceCell", typeof(GameObject)) as GameObject;
					Instantiate(iceCellPrefab, GameField.GetVectorFromCoord(TargetCoordinates.ElementAt(i).X, TargetCoordinates.ElementAt(i).Y),
							Quaternion.Euler(new Vector3()));
				}
				Destroy(iceBall.gameObject);
			}
			Game.PlayerIsBlocked = false;


		}
	}
}
