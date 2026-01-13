using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Cutscenes
{
    internal class EditorPatcher
    {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(EditorElement_CheckpointPanel), nameof(EditorElement_CheckpointPanel.OnAwake))]
        static void OnStart()
        {
            Plugin.checkpointName = EditorElement_CheckpointPanel.inst.left.Find("name/name").GetComponent<TMP_InputField>();

            //get "round vignette" toggle as template for "cutscene" toggle
            Plugin.toggle = UnityEngine.Object.Instantiate(
                EditorElement_EventPanel.inst.content.Find("vignette/settings/vignette-roundness/vignette-roundness-toggle").GetComponent<Toggle>(), 
                EditorElement_CheckpointPanel.inst.left, true);
            //set name
            Plugin.toggle.transform.GetChild(2).GetComponentInChildren<TextMeshProUGUI>().text = "Cutscene";
            Plugin.toggle.transform.GetChild(2).GetComponentInChildren<TextMeshProUGUI>().alignment = TextAlignmentOptions.Center;
            Plugin.toggle.name = "cutscene";
            //add listener
            Plugin.toggle.onValueChanged.RemoveAllListeners();
            Plugin.toggle.onValueChanged.AddListener(x =>
            {
                Plugin.checkpointName.interactable = !x;
                CheckpointEditor.inst.CurrentCheckpoint.name =
                    x ?
                    string.Concat("!CUTSCENE", CheckpointEditor.inst.CurrentCheckpoint.name) :
                    CheckpointEditor.inst.CurrentCheckpoint.name.Remove(0, 9);
            });
        }

        //Hide keyword in name field
        [HarmonyPostfix]
        [HarmonyPatch(typeof(EditorElement_CheckpointPanel), nameof(EditorElement_CheckpointPanel.RenderLeft))]
        static void RenderLeft()
        {
            Plugin.toggle.SetIsOnWithoutNotify(CheckpointEditor.inst.CurrentCheckpoint.name.Contains("!CUTSCENE"));
            Plugin.checkpointName.interactable = !Plugin.toggle.isOn;
            if (Plugin.toggle.isOn) Plugin.checkpointName.SetTextWithoutNotify(CheckpointEditor.inst.CurrentCheckpoint.name.Remove(0, 9));
        }

        //Hide keyword in checkpoints list
        [HarmonyPostfix]
        [HarmonyPatch(typeof(EditorElement_CheckpointPanel), nameof(EditorElement_CheckpointPanel.RenderCheckpointList))]
        static void RenderList()
        {
            foreach (Transform i in EditorElement_CheckpointPanel.inst.right.Find("checkpoints/viewport/content"))
            {
                var text = i.gameObject.transform.Find("name").GetComponent<TextMeshProUGUI>();
                if (text.text.Contains("!CUTSCENE")) text.text = text.text.Remove(0, 9);
            }
        }
    }
}