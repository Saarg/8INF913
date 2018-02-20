﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public abstract class Spell : NetworkBehaviour {

	[Header("Description")]
	[SerializeField] protected float lastActivation;
	[SerializeField] protected float manaCost;
	[SerializeField] protected float range;		//if range is 0, spell will be apply to caster
	[SerializeField] protected Living caster;
	[SerializeField] protected Targeting targetingSystem;
	[SerializeField] protected Vector3 target; //if necessary
	[SerializeField] protected Quaternion targetRotation;
	[SerializeField] protected KeyCode spellKey;
	[SerializeField] protected GameObject placeholder;

	[Header("Timers")]
	[SerializeField] protected float castingTime;
	[SerializeField] protected float cooldown;

	public ProgressBar castingBar { protected get; set;}

	protected virtual void Start(){
		lastActivation = cooldown;
	}

	protected virtual void Update(){
		lastActivation += Time.deltaTime;
	}

	public bool IsReady(){
		return (lastActivation >= cooldown);
	}

	protected virtual void DestroySpell(){
		GameObject.Destroy(this.gameObject);
	}

	[Command]
	public void CmdCast(){
		if (caster.isCasting) {
			Debug.Log ("Already casting");
			return;
		}
		if (caster.curMana < manaCost) {
			Debug.Log ("Not enough mana");
			return;
		}
		if (!IsReady ()) {
			Debug.Log ("Spell is not ready");
			return;
		}
		if (caster.isTargeting) {
			Debug.Log ("Already targeting for a spell");
			return;
		}

		RpcStartCasting();
	}

	[ClientRpc]
	void RpcStartCasting() {
		StartCoroutine (Casting ());
	}

	protected IEnumerator Casting(){
		castingBar.MinValue = 0;
		castingBar.MaxValue = castingTime;
		castingBar.StartValue = 0;
		castingBar.CurrentValue = 0;
		castingBar.Complete = false;

    caster.isCasting = true;
    castingBar.gameObject.SetActive (true);

    if (hasAuthority) {
      GetComponentInParent<Living> ().CmdApplyMoveStatus (MoveStatus.Casting);
    }

    if (range > 0) {
      caster.isTargeting = true;
      StartCoroutine (targetingSystem.AcquireTarget (range, spellKey));
    }

		StartCoroutine (Casting ());
	}

	protected IEnumerator Casting(){
		//wait for the target
		while (caster.isTargeting) {
			yield return 0;
		}

		if (!targetingSystem.Canceled ()) {
			caster.isCasting = true;
			this.GetComponent<Living> ().CmdApplyMoveStatus (MoveStatus.Casting);

			castingBar.gameObject.SetActive (true);

			//TODO add animation
			//TODO add sound

			if(range > 0){
				target = targetingSystem.getTarget (); //target is needed if range is not 0
				targetRotation = targetingSystem.getTargetRotation();
				placeholder = targetingSystem.getPlaceholder();
			}

			while(!castingBar.Complete){
				castingBar.Progress (Time.deltaTime);
				yield return 0;
			}

			ApplyEffect ();

			castingBar.gameObject.SetActive (false);
			caster.isCasting = false;
		}
	}

	public void ApplyEffect (){
		if (isServer)
			caster.curMana -= manaCost;

		Effects ();
		lastActivation = 0;
	}

	protected abstract void Effects();

	public KeyCode SpellKey(){ return spellKey; }
}
