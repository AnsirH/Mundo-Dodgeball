using UnityEngine;
using TMPro;
using DG.Tweening;
using System.Collections;

public class TopPanelUI : MonoBehaviour
{
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI[] scoreTexts; // 0: Player1, 1: Player2

    private float remainingTime;
    private bool timerRunning = false;
    private Color defaultColor = Color.white;
    private Coroutine blinkCoroutine;

    public void StartTimer(int minutes, int seconds)
    {
        remainingTime = minutes * 60 + seconds;
        timerRunning = true;
        UpdateTimerText();
    }

    private void Update()
    {
        if (!timerRunning) return;

        remainingTime -= Time.deltaTime;
        if (remainingTime <= 0)
        {
            remainingTime = 0;
            timerRunning = false;
            // 타이머 종료 처리 필요 시 여기에
        }

        UpdateTimerText();

        // 59초 이하부터 깜빡이기 시작
        if (remainingTime <= 59 && blinkCoroutine == null)
        {
            blinkCoroutine = StartCoroutine(BlinkTimerColor());
        }
    }

    private void UpdateTimerText()
    {
        int mins = Mathf.FloorToInt(remainingTime / 60);
        int secs = Mathf.FloorToInt(remainingTime % 60);
        timerText.text = $"{mins:00}:{secs:00}";
    }

    private IEnumerator BlinkTimerColor()
    {
        while (timerRunning && remainingTime > 0)
        {
            timerText.color = Color.red;
            yield return new WaitForSeconds(0.3f);
            timerText.color = defaultColor;
            yield return new WaitForSeconds(0.3f);
        }

        timerText.color = defaultColor;
        blinkCoroutine = null;
    }

    public void AddScoreToPlayer(int playerIndex)
    {
        if (playerIndex < 0 || playerIndex >= scoreTexts.Length) return;

        int score = int.Parse(scoreTexts[playerIndex].text);
        score++;
        scoreTexts[playerIndex].text = score.ToString();

        AnimateScore(scoreTexts[playerIndex]);
    }

    private void AnimateScore(TextMeshProUGUI scoreText)
    {
        scoreText.transform.DOComplete(); // 이전 애니메이션 중단
        scoreText.transform.DOPunchScale(Vector3.one * 0.5f, 0.3f, 5, 0.5f);
    }
}
