﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

/// <summary>  
/// 	Enum used to know the movement capacity of the entity
/// </summary>
public enum MoveStatus {Free, Ralenti, Casting, Immobilisé };  

/// <summary>  
/// 	Mother class of every living entity
/// </summary>
public class Living : NetworkBehaviour {

	[Header("Life")]
	public float maxLife = 100;

    [SyncVar(hook="UpdateLife")]
	public float curLife = 100f;

    [Header("Mana")]
    public float maxMana = 100;

    [SyncVar(hook = "UpdateMana")]
    public float curMana = 100f;

    [Header("Movement")]	
	public float speed = 1f;
    public float JumpSpeed = 1.0f;

    [Header("Etat movement")]
    public MoveStatus moveStatus;
    public bool canRun = true;
    public bool canJump = true;
    public bool canMove = false;
    public bool lowJump = false;

    [Header("Weakness/Strength")]
	[Range(0f, 2f)]
	public float fire = 1f;
	[Range(0f, 2f)]	
	public float ice = 1f;
	[Range(0f, 2f)]	
	public float lightning = 1f;
	[Range(0f, 2f)]	
	public float poison = 1f;
	[Range(0f, 2f)]	
	public float physical = 1f;

	// Events
	public delegate void DeathEvent();
    public event DeathEvent OnDeath;

	/// <summary>  
	/// 	curLife hook, sends death event if life goes to 0 and clamps the life between 0 and maxlife
	/// </summary>
	/// <remarks>
	/// 	Use this function to update entity's life
	/// </remarks>
	void UpdateLife(float life) {
		curLife = Mathf.Clamp(life, 0, maxLife);

		if (curLife == 0f) {
            if(OnDeath != null)
			    OnDeath();
            Destroy(gameObject);
		}
	}

    /// <summary>  
	/// 	curMana hook, clamps the mana between 0 and maxMana
	/// </summary>
	/// <remarks>
	/// 	Use this function to update entity's mana
	/// </remarks>
    public void UpdateMana(float mana)
    {
        curMana = Mathf.Clamp(mana, 0, maxMana);     
    }

	/// <summary>  
    /// 	Command to update move status
    /// </summary>
	[Command]
    public void CmdApplyMoveStatus(MoveStatus status)
    {
		RpcApplyMoveStatus(status);
	}

    /// <summary>  
    /// 	Rpc in charge of updating move status
    /// </summary>
	[ClientRpc]
	private void RpcApplyMoveStatus(MoveStatus status)
    {
        switch (moveStatus)
        {
            case MoveStatus.Free:
                canRun = true;
                canJump = true;
                canMove = true;
                lowJump = false;
                break;
            case MoveStatus.Ralenti:
                canRun = false;
                canJump = true;
                canMove = true;
                lowJump = false;
                break;
            case MoveStatus.Casting:
                canRun = true;
                canJump = false;
                canMove = true;
                lowJump = true;
                break;
            case MoveStatus.Immobilisé:
                canRun = false;
                canJump = false;
                canMove = false;
                lowJump = false;
                break;
            default:
                break;
        }
    }

    public void TakeDamage(int damage, Bullet.DamageTypeEnum damageType)
    {
        UpdateLife(curLife - CalculateResistance(damage, damageType));
		Debug.Log ("damage");
    }

    int CalculateResistance(int damage, Bullet.DamageTypeEnum damageType)
    {
        switch (damageType)
        {
            case Bullet.DamageTypeEnum.fire:
                return Mathf.FloorToInt((float)damage * fire);
            case Bullet.DamageTypeEnum.ice:
                return Mathf.FloorToInt((float)damage * ice);
            case Bullet.DamageTypeEnum.lightning:
                return Mathf.FloorToInt((float)damage * lightning);
            case Bullet.DamageTypeEnum.poison:
                return Mathf.FloorToInt((float)damage * poison);
			case Bullet.DamageTypeEnum.physical:
				return Mathf.FloorToInt((float)damage * physical);
            default:
                return damage;
        }
    }

    public float GetCurLife()
    {
        return curLife;
    }

    public float GetMaxLife()
    {
        return maxLife;
    }
}
