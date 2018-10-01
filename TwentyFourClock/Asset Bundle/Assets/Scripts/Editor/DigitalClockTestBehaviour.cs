using System;
using UnityEngine;
using UnityEngine.UI;

public class DigitalClockBehaviour : MonoBehaviour
{

	private int _alarmHour;
	private int _alarmMinute;
	private bool _alarmEnabled;
	private bool _alarmArmed;

	private bool _alarmGuiShow;

	private TextMesh _hourMinText;
	private TextMesh _secText;
	private TextMesh _alarmTimeText;
	private AudioSource _alarmSound;
	private Renderer _dotRenderer;
	private Renderer _alarmTimeRenderer;
	private Renderer _alarmIconRenderer;

	private GameObject _textParent;

	private Color _color;

	private DateTime _now;

	public void Arm(int h, int m)
	{
		_alarmHour = h % 24;
		_alarmMinute = m % 60;
		_alarmEnabled = true;
		Debug.Log(string.Format("Alarm set: {0}:{1}", h, m));
	}

	public void Disarm()
	{
		_alarmSound.Stop();
		_alarmEnabled = false;
	}

	private void DrawGui()
	{
		GUILayout.BeginArea(new Rect(Screen.width / 2 - 100, Screen.height - 220, 200, 170), GUI.skin.box);
		GUILayout.BeginVertical();

		// Hour
		GUILayout.Label("Hour");
		int.TryParse(GUILayout.TextField(_alarmHour.ToString()), out _alarmHour);
		_alarmHour %= 24;
		GUILayout.Label("Minute");
		int.TryParse(GUILayout.TextField(_alarmMinute.ToString()), out _alarmMinute);
		_alarmMinute %= 60;
		GUILayout.Label("Color");
		GUILayout.BeginHorizontal();
		_color.r = GUILayout.HorizontalSlider(_color.r, 0, 1);
		_color.g = GUILayout.HorizontalSlider(_color.g, 0, 1);
		_color.b = GUILayout.HorizontalSlider(_color.b, 0, 1);
		GUILayout.EndHorizontal();
		GUILayout.EndVertical();

		// Buttons
		GUILayout.BeginHorizontal();
		if (GUILayout.Button("Arm"))
		{
			Arm(_alarmHour, _alarmMinute);
			_alarmGuiShow = false;
		}
		if (GUILayout.Button("Disarm"))
		{
			Disarm();
			_alarmGuiShow = false;
		}
		if (GUILayout.Button("Close"))
		{
			_alarmGuiShow = false;
		}
		GUILayout.EndHorizontal();
		GUILayout.EndArea();

		foreach (MeshRenderer c in _textParent.GetComponentsInChildren<MeshRenderer>())
		{
			c.material.SetColor("_Color", _color);
			c.material.SetColor("_Emissive", _color);
		}
	}

	void Awake()
	{
		_color = Color.red;
		_now = DateTime.Now;
		_textParent = gameObject.transform.Find("digitalclock_text").gameObject;
		_hourMinText = gameObject.transform.Find("digitalclock_text/digitalclock_hourmin").gameObject.GetComponent<TextMesh>();
		_secText = gameObject.transform.Find("digitalclock_text/digitalclock_sec").gameObject.GetComponent<TextMesh>();
		_alarmTimeText = gameObject.transform.Find("digitalclock_text/digitalclock_alarm_time").gameObject.GetComponent<TextMesh>();

		_dotRenderer = gameObject.transform.Find("digitalclock_text/digitalclock_blink").gameObject.GetComponent<Renderer>();
		_alarmIconRenderer = gameObject.transform.Find("digitalclock_text/digitalclock_alarm_icon").gameObject.GetComponent<Renderer>();
		_alarmTimeRenderer = gameObject.transform.Find("digitalclock_text/digitalclock_alarm_time").gameObject.GetComponent<Renderer>();

		_alarmSound = gameObject.transform.Find("digitalclock_beep").gameObject.GetComponent<AudioSource>();
	}

	void Update()
	{
		_now = DateTime.Now;

		if (Input.GetKeyDown(KeyCode.V))
		{
			Arm(_now.Hour, _now.Minute + 1);
		}
		if (Input.GetKeyDown(KeyCode.F))
		{
			if (_alarmSound.isPlaying)
				_alarmSound.Stop();
			else
				_alarmGuiShow = true;
		}

		if (_alarmArmed && _now.Hour == _alarmHour && _now.Minute == _alarmMinute)
		{
			_alarmSound.Play();
		}
		_alarmArmed = !(_now.Hour == _alarmHour && _now.Minute == _alarmMinute);
		if (Input.GetKeyDown(KeyCode.X) && _alarmSound.isPlaying)
		{
			_alarmEnabled = false;
			_alarmSound.Stop();
		}
	}

	void OnGUI()
	{
		_hourMinText.text = _now.ToString("H:mm");
		_secText.text = _now.ToString("ss");
		_dotRenderer.enabled = _now.Millisecond < 500;
		_alarmTimeRenderer.enabled = _alarmIconRenderer.enabled = _alarmEnabled;
		if (_alarmEnabled)
		{
			_alarmTimeText.text = string.Format("{0}:{1}", _alarmHour.ToString("00"), _alarmMinute.ToString("00"));
		}

		if (_alarmGuiShow)
			DrawGui();
	}
}
