using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HealthAndShieldDisplay : MonoBehaviour 
{
	public Stats playerStats;
	public void Start()
	{
		StartCoroutine(CoUpdate() );
	}
	
	private float lastHealthChecked = 0;
	private float lastArmorChecked = 0;
	
	private float lastCheckedHealthAmount = 0;
	private float lastCheckArmorAmount = 0;
	
	public Transform ArmorControlTrans;
	public Transform HealthControlTrans;
	
	private List<GameObject> HealthControl = new List<GameObject>();
	private List<GameObject> ArmorControl = new List<GameObject>();
	public void OnEnable()
	{
		for(int i = 0; i < HealthControlTrans.GetChildCount(); i++)
		{
			HealthControl.Add(HealthControlTrans.GetChild(i).gameObject );
			if(playerStats.Health <= i)
			{
				HealthControl[i].SetActiveRecursively(false);
			}
			else
			{
				HealthControl[i].SetActiveRecursively(true);
			}
		}
		
		for(int i = 0; i < ArmorControlTrans.GetChildCount(); i++)
		{
			ArmorControl.Add(ArmorControlTrans.GetChild(i).gameObject );
			if(playerStats.Armor <= i)
			{
				ArmorControl[i].SetActiveRecursively(false);
			}
			else
			{
				ArmorControl[i].SetActiveRecursively(true);
			}
		}
	}
	
	public IEnumerator CoUpdate()
	{
		while(true)
		{
			if(playerStats.Health != lastHealthChecked)
			{
				for(int i = 0; i < HealthControlTrans.GetChildCount(); i++)
				{
					if(playerStats.Health <= i)
					{
						HealthControl[i].SetActiveRecursively(false);
					}
					else
					{
						HealthControl[i].SetActiveRecursively(true);
					}
				}
				lastHealthChecked = playerStats.Health;
			}
			
			if(playerStats.Armor != lastArmorChecked)
			{
				for(int i = 0; i < ArmorControlTrans.GetChildCount(); i++)
				{
					if(playerStats.Armor <= i)
					{
						ArmorControl[i].SetActiveRecursively(false);
					}
					else
					{
						ArmorControl[i].SetActiveRecursively(true);
					}
				}
				lastArmorChecked = playerStats.Armor;
			}
			
			/** Actually Update the Slider Values of the displayed Indicator's to the correct values*/
			if(playerStats.GetHealth() != lastCheckedHealthAmount)
			{
				//Only get the last reference of the armor instead of all.
				int location = Mathf.CeilToInt(playerStats.GetHealth() - 1);
				float sliderVal = playerStats.GetHealth()- Mathf.FloorToInt( playerStats.GetHealth());
				if(sliderVal == 0)
				{
					location++;
				}
				for(int i = 0; i <= HealthControl.Count; i++)
				{
					if(i >= HealthControl.Count)
					{
						//Do nothing
					}
					else
					{
						UISlider healthSlider = GetSliderComponent( HealthControl[i] );
						
						if(playerStats.Health == location)			//All Bars are full
						{
							healthSlider.sliderValue = 1;
						}	
						else
						{
							if(i > location)
							{
								healthSlider.sliderValue = 0;
							}
							else if(location == i)						//Our currently effected cross
							{
								healthSlider.sliderValue = sliderVal;
							}	
							else
							{
								healthSlider.sliderValue = 1;
							}
						}
					}
				}
				lastCheckedHealthAmount = playerStats.GetHealth() ;
			}
			
			if(playerStats.GetArmor() != lastCheckArmorAmount)
			{
				
				//Only get the last reference of the armor instead of all.
				int location = Mathf.CeilToInt(playerStats.GetArmor() - 1);
				float sliderVal = playerStats.GetArmor()- Mathf.FloorToInt( playerStats.GetArmor());
				if(sliderVal == 0)
				{
					location++;
				}
				for(int i = 0; i <= location; i++)
				{
					if(i >= ArmorControl.Count)
					{
						//Do nothing
					}
					else
					{
						UISlider armorSlider = GetSliderComponent( ArmorControl[i] );
						if(playerStats.Armor == location)			//All Bars are full
						{
							armorSlider.sliderValue = 1;
						}	
						else
						{
							if(location == i)		//Our currently effected cross
							{
								armorSlider.sliderValue = sliderVal;
							}	
							else
								armorSlider.sliderValue = 1;
						}
					}
				}
				lastCheckArmorAmount = playerStats.GetArmor() ;
			}
			yield return new WaitForSeconds(0.1f);
		}
	}
	
	public UISlider GetSliderComponent(GameObject go)
	{
		return go.GetComponent(typeof(UISlider)) as UISlider;
	}
}
