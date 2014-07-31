using System;
using System.Collections;
using UnityEngine;

namespace AssemblyCSharp
{
	public class InputManager : MonoBehaviour
	{
		private static InputManager instance;

		public bool allowMouse = true;
		public bool allowController = true;
		public float deadZoneValue = 0.3f;

		private bool usingMouse = true;

		public delegate void OnControlTypeChangedDelegate(bool changedToMouse);
		public static event OnControlTypeChangedDelegate OnControlTypeChangedEvent;

		public bool UsingMouse { get { return usingMouse; } }

		public InputManager ()
		{
			
		}
		
		public static InputManager Instance
		{
			get;
			private set;
		}
		
		void Awake()
		{
			if (Instance != null && Instance != this)
			{
				Destroy(gameObject);
			}
			
			Instance = this;	
			DontDestroyOnLoad(gameObject);
		}
		
		void Start()
		{
			if (!allowMouse && allowController)
				usingMouse = false;

			TopDownAim[] topDownAimComponents = FindObjectsOfType(typeof(TopDownAim)) as TopDownAim[];

			for (int i = 0; i < topDownAimComponents.Length; i++)
				topDownAimComponents[i].deadZoneValue = deadZoneValue;
		}
		
		void Update()
		{
			float controllerXValue = Input.GetAxis("RightStickX");
			float controllerYValue = Input.GetAxis("RightStickY");
			
			if (allowMouse && (Mathf.Abs(Input.GetAxis("Mouse X")) > 5 || Mathf.Abs(Input.GetAxis("Mouse Y")) > 0.3f))
				SwitchInputType(true);
			else if (allowController && ((Mathf.Abs(controllerXValue) >= deadZoneValue || Mathf.Abs(controllerYValue) >= deadZoneValue)))
				SwitchInputType(false);
		}

		public void OnApplicationQuit()
		{
			instance = null;	
		}
		
		protected void SwitchInputType(bool toMouse)
		{
			if (toMouse && !usingMouse)
			{
				usingMouse = true;
				OnControlTypeChangedEvent(true);
			}
			else if (!toMouse && usingMouse)
			{
				usingMouse = false;
				OnControlTypeChangedEvent(false);
			}
		}
	}
}

