﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionSpell : Spell {

	[SerializeField] private GameObject explosionPrefab;

	protected override void Effects(){
		Instantiate(explosionPrefab, target, Quaternion.Euler(Vector3.zero));
	}
}
