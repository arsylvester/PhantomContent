﻿/*

The MIT License (MIT)

Copyright (c) 2015-2017 Secret Lab Pty. Ltd. and Yarn Spinner contributors.

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.

*/

using UnityEngine;

namespace Yarn.Unity.Example {
    /// attached to the non-player characters, and stores the name of the Yarn
    /// node that should be run when you talk to them.

    public class NPC : MonoBehaviour {

        public string characterName = "";

        [SerializeField] string talkToNode = "";

        [Header("Optional")]
        public YarnProgram scriptToLoad;

        [Header("Other")]
        public AudioClip[] speakToClips;
        private AudioSource audio;
        [SerializeField] TextMesh exclamationPoint;
        [SerializeField] float exclamationSize = 2;
        private bool hasTalkedTo = false;
        private Transform player;

        void Start () {
            if (scriptToLoad != null) {
                DialogueRunner dialogueRunner = FindObjectOfType<Yarn.Unity.DialogueRunner>();
                dialogueRunner.Add(scriptToLoad);                
            }

            player = FindObjectOfType<PlayerController>().transform;
            audio = GetComponent<AudioSource>();
            hasTalkedTo = PlayerPrefs.HasKey(talkToNode);
        }

        private void Update()
        {
            float distance = Vector3.Distance(transform.position, player.position) * exclamationSize;
            exclamationPoint.fontSize = (int)distance;
        }

        public string GetTalkToNode()
        {
            audio.PlayOneShot(speakToClips[Random.Range(0, speakToClips.Length - 1)]);
            hasTalkedTo = true;
            PlayerPrefs.SetInt("hasTalkedTo", 1);
            PlayerPrefs.Save();
            exclamationPoint.gameObject.SetActive(false);
            return talkToNode;
        }

        public void SetExclamationPoint(bool value)
        {
            if(!hasTalkedTo)
                exclamationPoint.gameObject.SetActive(value);
        }
    }

}
