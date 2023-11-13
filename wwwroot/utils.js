function ScrollChat() {
    window.scrollTo(0, document.body.scrollHeight)
}


function PlayAudio (url, vol = 0.2) {
    const audio = new Audio(url);
    audio.volume = vol;
    audio.play();
}

function GetTTSVoices() {
    return window.speechSynthesis.getVoices();
}

function TTS(text, vol = 0.2, voice = undefined) {
    var msg = new SpeechSynthesisUtterance();
    var voices = window.speechSynthesis.getVoices();
    msg.voice = voice ? voice : voices[0];
    msg.volume = vol;
    msg.text = text;
    window.speechSynthesis.speak(msg);
}

function AppendCss(id, css) {
    var style = document.createElement('style');
    style.textContent = css;
    document.getElementById(id).contentDocument.head.appendChild(style);
}

function FormatCode(Code) {
    return hljs.highlightAuto(Code).value;
}
