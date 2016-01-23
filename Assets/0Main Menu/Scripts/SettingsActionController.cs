using UnityEngine;
using UnityEngine.UI;

public class SettingsActionController : ActionContentController
{
	//////////////////////////////////////////////////////////////
	///////////////////////// COMPONENTS /////////////////////////
	//////////////////////////////////////////////////////////////

	public Button okButton;
	public Button cancelButton;

	public Toggle[] levelsToggles;
	public Toggle particlesToggle;
	public Toggle dofToggle;
	public Toggle acToggle;
	public Slider soundSlider;

	//////////////////////////////////////////////////////////////
	///////////////////// CONTROL VARIABLES //////////////////////
	//////////////////////////////////////////////////////////////

	private int selectedIndex;
	private int realIndex;

	////////////////////////////////////////////////////////////////////////////////////////////////
	//////////////////////////////////////// MONO BEHAVIOR /////////////////////////////////////////
	////////////////////////////////////////////////////////////////////////////////////////////////

	void Start()
	{
		selectedIndex = CubilineApplication.singleton.settings.qualityIndex;
		realIndex = selectedIndex;
		levelsToggles[selectedIndex].isOn = true;
		particlesToggle.isOn = CubilineApplication.singleton.settings.particles;
		dofToggle.isOn = CubilineApplication.singleton.settings.depthOfField;
		acToggle.isOn = CubilineApplication.singleton.settings.ambientOcclusion;
		soundSlider.value = CubilineApplication.singleton.settings.masterSoundLevel * 100;
	}

	public override void Select()
	{
		okButton.GetComponent<EaseImageOpasity>().easeFace = EaseFloat.EASE_FACE.IN;
		okButton.transform.GetChild(0).GetComponent<EaseTextOpasity>().easeFace = EaseFloat.EASE_FACE.IN;
		cancelButton.GetComponent<EaseImageOpasity>().easeFace = EaseFloat.EASE_FACE.IN;
		cancelButton.transform.GetChild(0).GetComponent<EaseTextOpasity>().easeFace = EaseFloat.EASE_FACE.IN;
		GetComponent<GraphicRaycaster>().enabled = true;
	}

	public override void Unselect()
	{
		okButton.GetComponent<EaseImageOpasity>().easeFace = EaseFloat.EASE_FACE.OUT;
		okButton.transform.GetChild(0).GetComponent<EaseTextOpasity>().easeFace = EaseFloat.EASE_FACE.OUT;
		cancelButton.GetComponent<EaseImageOpasity>().easeFace = EaseFloat.EASE_FACE.OUT;
		cancelButton.transform.GetChild(0).GetComponent<EaseTextOpasity>().easeFace = EaseFloat.EASE_FACE.OUT;
		GetComponent<GraphicRaycaster>().enabled = false;
		base.Unselect();
	}

	public override void Enter()
	{

	}

	public override void Leave()
	{
		
	}

	public void SetQualityLevel(string index)
	{
		selectedIndex = int.Parse(index);
		CubilineApplication.singleton.settings.qualityIndex = selectedIndex;
		QualitySettings.SetQualityLevel(selectedIndex, true);
	}

	public void SetAudioLevel(float level)
	{
		AudioListener.volume = level / 100.0f;
	}

	public void OkAction()
	{
		realIndex = selectedIndex;
		CubilineApplication.singleton.settings.qualityIndex = selectedIndex;
		CubilineApplication.singleton.settings.particles = particlesToggle.isOn;
		CubilineApplication.singleton.settings.depthOfField = dofToggle.isOn;
		CubilineApplication.singleton.settings.ambientOcclusion = acToggle.isOn;
		CubilineApplication.singleton.settings.masterSoundLevel = soundSlider.value / 100.0f;
		CubilineApplication.singleton.SaveSettings();
		Unselect();
	}

	public void CancelAction()
	{
		selectedIndex = realIndex;
		CubilineApplication.singleton.settings.qualityIndex = realIndex;
		QualitySettings.SetQualityLevel(selectedIndex, true);
		levelsToggles[selectedIndex].isOn = true;
		particlesToggle.isOn = CubilineApplication.singleton.settings.particles;
		dofToggle.isOn = CubilineApplication.singleton.settings.depthOfField;
		acToggle.isOn = CubilineApplication.singleton.settings.ambientOcclusion;
		soundSlider.value = CubilineApplication.singleton.settings.masterSoundLevel * 100;
		Unselect();
	}
}
