using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cysharp.Threading.Tasks;
using UnityEngine;
using BlackTree.Definition;

namespace BlackTree.Core
{
    public class StaticData
    {
        public static StaticDataWrapper Wrapper { get; private set; }
      
        public static async UniTask Load()
        {
#if UNITY_EDITOR
            var dataCommonType = typeof(StaticDataWrapper);
            var dataCommonFields = dataCommonType.GetFields();
            StringBuilder sb = new StringBuilder(1000);
            sb.Append("{");
      
            foreach (var dataCommonField in dataCommonFields)
            {
                var filename = $"{dataCommonField.Name}";
                var filepath = $"JsonData/{filename}";
                sb.Append('\"');
                sb.Append(dataCommonField.Name);
                sb.Append('\"');
                sb.Append(':');
                Debug.Log(filepath);
                sb.Append(FileRead(filepath));
                sb.Append(',');
            }
            sb.Remove(sb.Length - 1, 1);
            sb.Append("}");

            Wrapper=JsonUtility.FromJson<StaticDataWrapper>(sb.ToString());

#else
            var tempWrapper =await FirebaseRD.LoadTableDataFromFirebase();
            Wrapper = tempWrapper;//

#endif

            if (Wrapper == null)
                return;
            foreach (var seq in Wrapper.upgradeSequences)
            {
                seq.CreateCache();
            }

            foreach (var seq in Wrapper.researchTableSequence)
            {
                seq.CreateCache();
            }
        }

        static string FileRead(string path)
        {
            TextAsset jsonString = Resources.Load<TextAsset>(path.ToString());

            return jsonString.text;
        }
    }

}
