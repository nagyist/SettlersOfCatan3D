﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Player : MonoBehaviour {

	public string playerName;
	public Color playerColor;

	public int playerNumber;
	public int goldCoins;
	public int victoryPoints;

	// private Dictionary<System.Type, List<Unit>> ownedUnits -> ownedUnits[typeof(settlement)].add(settlement) -> O(1) access to list of specific type of units
	private Dictionary<System.Type, List<Unit>> ownedUnits;
	public ResourceTuple resources = new ResourceTuple(20, 20, 20, 20, 20);
	public CommodityTuple commodities;

	public Edge lastEdgeSelection;
	public Intersection lastIntersectionSelection;
	public int lastUnitSelectionId;

	// Use this for initialization
	void Start () {
		//playerColor = Color.black;
		initializeUnitsDictionary();
	}
	
	// Update is called once per frame
	void Update () {
		victoryPoints = getTotalVictoryPoints ();

		if (victoryPoints >= GameManager.victoryPointsToWinGame) {
			winGame ();
		}
	}

	void initializeUnitsDictionary() {
		ownedUnits = new Dictionary<System.Type, List<Unit>>();

		ownedUnits.Add(typeof(Settlement), new List<Unit>());
		ownedUnits.Add(typeof(City), new List<Unit>());
		ownedUnits.Add(typeof(Metropolis), new List<Unit>());
		ownedUnits.Add(typeof(CityWall), new List<Unit>());
		ownedUnits.Add(typeof(Road), new List<Unit>());
		ownedUnits.Add(typeof(Ship), new List<Unit>());
		ownedUnits.Add(typeof(Knight), new List<Unit>());

		//ownedUnits.Add (typeof(IntersectionUnit), new List<IntersectionUnit> ());
		//ownedUnits.Add (typeof(TradeUnit), new List<TradeUnit> ());

		ownedUnits.Add (typeof(Unit), new List<Unit> ());
	}

	public void addOwnedUnit(Unit unit, System.Type type) {
		bool defaultCase = false;

		switch (type.ToString()) {
		case "Settlement":
			ownedUnits [typeof(Settlement)].Add ((Settlement)unit);
			break;
		case "City":
			ownedUnits [typeof(City)].Add ((City)unit);
			break;
		case "Metropolis":
			ownedUnits [typeof(Metropolis)].Add ((Metropolis)unit);
			break;
		case "CityWall":
			ownedUnits [typeof(CityWall)].Add ((CityWall)unit);
			break;
		case "Road":
			ownedUnits [typeof(Road)].Add ((Road)unit);
			break;
		case "Ship":
			ownedUnits [typeof(Ship)].Add ((Ship)unit);
			break;
		case "Knight":
			ownedUnits [typeof(Knight)].Add ((Knight)unit);
			break;
		default:
			ownedUnits [typeof(Unit)].Add (unit);
			defaultCase = true;
			break;
		}

		if (!defaultCase) {
			ownedUnits [typeof(Unit)].Add (unit);
		}
	}

	public void removeOwnedUnit(Unit unit, System.Type type) {
		bool defaultCase = false;

		switch (type.ToString()) {
		case "Settlement":
			ownedUnits [typeof(Settlement)].Remove ((Settlement)unit);
			break;
		case "City":
			ownedUnits [typeof(City)].Remove ((City)unit);
			break;
		case "Metropolis":
			ownedUnits [typeof(Metropolis)].Remove ((Metropolis)unit);
			break;
		case "CityWall":
			ownedUnits [typeof(CityWall)].Remove ((CityWall)unit);
			break;
		case "Road":
			ownedUnits [typeof(Road)].Remove ((Road)unit);
			break;
		case "Ship":
			ownedUnits [typeof(Ship)].Remove ((Ship)unit);
			break;
		case "Knight":
			ownedUnits [typeof(Knight)].Remove ((Knight)unit);
			break;
		default:
			ownedUnits [typeof(Unit)].Remove (unit);
			defaultCase = true;
			break;
		}

		if (!defaultCase) {
			ownedUnits [typeof(Unit)].Remove (unit);
		}
	}

	public void addOwnedUnit(Settlement unit) {
		ownedUnits [typeof(Settlement)].Add (unit);
		ownedUnits [typeof(Unit)].Add (unit);
	}

	public void addOwnedUnit(City unit) {
		ownedUnits [typeof(City)].Add (unit);
		ownedUnits [typeof(Unit)].Add (unit);
	}

	public void addOwnedUnit(Metropolis unit) {
		ownedUnits [typeof(Metropolis)].Add (unit);
		ownedUnits [typeof(Unit)].Add (unit);
	}

	public void addOwnedUnit(CityWall unit) {
		ownedUnits [typeof(CityWall)].Add (unit);
		ownedUnits [typeof(Unit)].Add (unit);
	}

	public void addOwnedUnit(Road unit) {
		ownedUnits [typeof(Road)].Add (unit);
		ownedUnits [typeof(Unit)].Add (unit);
	}

	public void addOwnedUnit(Ship unit) {
		ownedUnits [typeof(Ship)].Add (unit);
		ownedUnits [typeof(Unit)].Add (unit);
	}

	public void addOwnedUnit(Knight unit) {
		ownedUnits [typeof(Knight)].Add (unit);
		ownedUnits [typeof(Unit)].Add (unit);
	}

	//public void addOwnedUnit(Unit unit) {
	//	ownedUnits [typeof(Unit)].Add (unit);
	//}

	#region Resource Receive/Give Methods

	public void receiveResources(ResourceTuple resourceToAdd) {
		List<ResourceType> resourceKeys = new List<ResourceType>(resources.resourceTuple.Keys);

		for (int i = 0; i < resourceKeys.Count; i++) {
			if (resourceToAdd.resourceTuple [resourceKeys [i]] >= 0) {
				print ("Added " + resourceToAdd.resourceTuple [resourceKeys [i]].ToString () + " " + resourceKeys [i].ToString () + " to " + this.playerName);
				resources.resourceTuple [resourceKeys [i]] += resourceToAdd.resourceTuple [resourceKeys [i]];
			}
		}
	}

	public void spendResources(ResourceTuple resourceToSpend) {
		List<ResourceType> resourceKeys = new List<ResourceType>(resources.resourceTuple.Keys);

		for (int i = 0; i < resourceKeys.Count; i++) {
			if (resources.resourceTuple [resourceKeys [i]] >= resourceToSpend.resourceTuple [resourceKeys [i]]) {
				print ("Subtracted " + resourceToSpend.resourceTuple [resourceKeys [i]].ToString () + " " + resourceKeys [i].ToString () + " from " + this.playerName);
				resources.resourceTuple [resourceKeys [i]] -= resourceToSpend.resourceTuple [resourceKeys [i]];
			}
		}
	}

	public bool hasAvailableResources(ResourceTuple resourcesNeeded) {
		List<ResourceType> resourceKeys = new List<ResourceType>(resources.resourceTuple.Keys);

		for (int i = 0; i < resourceKeys.Count; i++) {
			if (resources.resourceTuple [resourceKeys [i]] < resourcesNeeded.resourceTuple [resourceKeys [i]]) {
				return false;
			}
		}

		return true;
	}

	public void receiveResource(ResourceType resource, int amount) {
		if (amount < 0) {
			//Error
		} else {
			resources.resourceTuple [resource] += amount;
		}
	}

	public ResourceTuple getCurrentResources() {
		return resources;
	}

	#endregion

	#region Commodity Receive/Give Methods

	public void receiveCommodities(CommodityTuple commodityToAdd) {
		List<CommodityType> commodityKeys = new List<CommodityType>(commodities.commodityTuple.Keys);

		for (int i = 0; i < commodityKeys.Count; i++) {
			if (commodityToAdd.commodityTuple [commodityKeys [i]] >= 0) {
				print ("Added " + commodityToAdd.commodityTuple [commodityKeys [i]].ToString () + " " + commodityKeys [i].ToString () + " to " + this.playerName);
				commodities.commodityTuple [commodityKeys [i]] += commodityToAdd.commodityTuple [commodityKeys [i]];
			}
		}
	}

	public void spendCommodities(CommodityTuple commodityToSpend) {
		List<CommodityType> commodityKeys = new List<CommodityType>(commodities.commodityTuple.Keys);

		for (int i = 0; i < commodityKeys.Count; i++) {
			if (commodities.commodityTuple [commodityKeys [i]] >= commodityToSpend.commodityTuple [commodityKeys [i]]) {
				print ("Subtracted " + commodityToSpend.commodityTuple [commodityKeys [i]].ToString () + " " + commodityKeys [i].ToString () + " from " + this.playerName);
				commodities.commodityTuple [commodityKeys [i]] -= commodityToSpend.commodityTuple [commodityKeys [i]];
			}
		}
	}

	public bool hasAvailableCommodities(CommodityTuple commoditiesNeeded) {
		List<CommodityType> commodityKeys = new List<CommodityType>(commodities.commodityTuple.Keys);

		for (int i = 0; i < commodityKeys.Count; i++) {
			if (commodities.commodityTuple [commodityKeys [i]] < commoditiesNeeded.commodityTuple [commodityKeys [i]]) {
				return false;
			}
		}

		return true;
	}

	public void receiveCommodity(CommodityType commodity, int amount) {
		if (amount < 0) {
			//Error
		} else {
			commodities.commodityTuple [commodity] += amount;
		}
	}

	public CommodityTuple getCurrentCommodities() {
		return commodities;
	}

	#endregion

	public void playTurn() {

	}

	private void winGame() {
		// TODO
	}

	private int getTotalVictoryPoints() {
		List<Unit> unitsOwned = ownedUnits [typeof(Unit)];
		int totalVP = 0;
		for(int i = 0; i < unitsOwned.Count; i++) {
			totalVP += unitsOwned [i].victoryPointsWorth;
		}
		return totalVP;
	}

	public List<Unit> getOwnedUnitsOfType(System.Type type) {
		print ("Getting type: " + type.ToString ());

		switch (type.ToString()) {
		case "Settlement":
			print ("In Settlement");
			return ownedUnits [typeof(Settlement)];
		case "City":
			print ("In City");
			return ownedUnits [typeof(City)];
		case "Metropolis":
			print ("In Metropolis");
			return ownedUnits [typeof(Metropolis)];
		case "CityWall":
			print ("In CityWall");
			return ownedUnits [typeof(CityWall)];
		case "Road":
			print ("In Road");
			return ownedUnits [typeof(Road)];
		case "Ship":
			print ("In Ship");
			return ownedUnits [typeof(Ship)];
		case "Knight":
			print ("In Knight");
			return ownedUnits [typeof(Knight)];
		default:
			print ("Default add case");
			return ownedUnits [typeof(Unit)];
		}
	}

	public List<Unit> getOwnedUnits() {
		List<Unit> clone = new List<Unit> (ownedUnits[typeof(Unit)]);
		return clone;
	}

	public List<Settlement> getOwnedUnitsOfThisType(Settlement settlement) {
		return ownedUnits [typeof(Settlement)].Cast<Settlement> ().ToList ();
	}

	public List<City> getOwnedUnitsOfThisType(City city) {
		return ownedUnits [typeof(City)].Cast<City> ().ToList ();
	}

	public List<Metropolis> getOwnedUnitsOfThisType(Metropolis unit) {
		return ownedUnits [typeof(Metropolis)].Cast<Metropolis> ().ToList ();
	}

	public List<CityWall> getOwnedUnitsOfThisType(CityWall unit) {
		return ownedUnits [typeof(CityWall)].Cast<CityWall> ().ToList ();
	}

	public List<Road> getOwnedUnitsOfThisType(Road unit) {
		return ownedUnits [typeof(Road)].Cast<Road> ().ToList ();
	}

	public List<Ship> getOwnedUnitsOfThisType(Ship unit) {
		return ownedUnits [typeof(Ship)].Cast<Ship> ().ToList ();
	}

	public List<Knight> getOwnedUnitsOfThisType(Knight unit) {
		return ownedUnits [typeof(Knight)].Cast<Knight> ().ToList ();
	}

	public int getNumUnits() {
		return ownedUnits[typeof(Unit)].Count;
	}

	public IEnumerator makeEdgeSelection(List<Edge> possibleEdges) {//, TradeUnit unitToBuild) {
		bool selectionMade = false;
		Edge selectedEdge;

		while (!selectionMade) {
			yield return StartCoroutine (GameEventHandler.WaitForKeyDown (KeyCode.Mouse0));

			RaycastHit hitInfo = new RaycastHit();
			bool hit = Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo);

			if (hit) {
				if (hitInfo.transform.gameObject.tag == "Edge") {
					selectedEdge = hitInfo.transform.gameObject.GetComponent<Edge>();

					if (selectedEdge != null && selectedEdge.occupier == null && possibleEdges.Contains(selectedEdge)) {//(selectedEdge.isLandEdge() || selectedEdge.isShoreEdge())) {
						selectionMade = true;
						lastEdgeSelection = selectedEdge;
					}
				} 
			}
		}
	}

	public IEnumerator makeIntersectionSelection(List<Intersection> possibleIntersections) {
		bool selectionMade = false;
		Intersection selectedIntersection;

		while (!selectionMade) {
			yield return StartCoroutine (GameEventHandler.WaitForKeyDown (KeyCode.Mouse0));

			RaycastHit hitInfo = new RaycastHit();
			bool hit = Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo);

			if (hit) {
				if (hitInfo.transform.gameObject.tag == "Intersection") {
					
					selectedIntersection = hitInfo.transform.gameObject.GetComponent<Intersection>();

					if (selectedIntersection != null && selectedIntersection.occupier == null && possibleIntersections.Contains(selectedIntersection)) {
						selectionMade = true;
						lastIntersectionSelection = selectedIntersection;
					}
				}
			} 
		}
	}

	public IEnumerator makeUnitSelection(List<Unit> possibleUnits) {
		bool selectionMade = false;
		Unit selectedUnit;

		while (!selectionMade) {
			yield return StartCoroutine (GameEventHandler.WaitForKeyDown (KeyCode.Mouse0));

			RaycastHit hitInfo = new RaycastHit();
			bool hit = Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo);

			if (hit) {
				if (hitInfo.transform.parent.gameObject.tag == "Unit") {
					selectedUnit = hitInfo.transform.gameObject.GetComponentInParent<Unit>();

					if (selectedUnit != null && possibleUnits.Contains(selectedUnit)) {
						selectionMade = true;
						lastUnitSelectionId = selectedUnit.id;
					}
				}
			} 
		}
	}
}
