using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Match3Game.Presentation.Animation
{
    /// <summary>
    /// Birden fazla animasyonu sıraya koymak için yardımcı.
    /// BoardController'daki Coroutine karmaşıklığını azaltır.
    /// </summary>
    public class AnimationSequencer
    {
        private readonly Queue<Action<Action>> _queue = new();
        private bool _isRunning;

        public AnimationSequencer Append(Action<Action> animation)
        {
            _queue.Enqueue(animation);
            return this;
        }

        public void Play(MonoBehaviour runner, Action onComplete = null)
        {
            if (_isRunning) return;
            runner.StartCoroutine(RunSequence(onComplete));
        }

        private IEnumerator RunSequence(Action onComplete)
        {
            _isRunning = true;

            while (_queue.Count > 0)
            {
                bool done = false;
                var step = _queue.Dequeue();
                step(() => done = true);
                yield return new WaitUntil(() => done);
            }

            _isRunning = false;
            onComplete?.Invoke();
        }
    }
}

