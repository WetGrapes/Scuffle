using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StaticStorageInstaller : SerializedMonoBehaviour
{
    
    public IStaticStorage[] StaticStorages = new IStaticStorage[0];
    void OnEnable()
    {
        foreach (var storage in StaticStorages) storage.SetActive(true);
    }
    void OnDisable()
    {
        foreach (var storage in StaticStorages) storage.SetActive(false);
    }
}
