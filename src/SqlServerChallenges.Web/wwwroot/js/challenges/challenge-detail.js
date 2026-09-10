import {getEditorValue, setSyntaxErrors, clearSyntaxErrors} from './challenge-editor.js';

function applySyntaxMarkers() {
    const el = document.querySelector('#challenge-detail__result-body [data-syntax-errors]');
    if (!el) return;
    try {
        const errors = JSON.parse(el.dataset.syntaxErrors);
        setSyntaxErrors(errors);
    } catch {}
}

function toggleVote(type) {
    const upBtn = document.getElementById('vote-up');
    const downBtn = document.getElementById('vote-down');
    const countSpan = upBtn.querySelector('.challenge-detail__vote-count');
    const count = parseInt(countSpan.textContent, 10);

    if (type === 'up') {
        const toggledOn = upBtn.classList.toggle('challenge-detail__vote-btn--clicked-up');
        downBtn.classList.remove('challenge-detail__vote-btn--clicked-down');
        countSpan.textContent = toggledOn ? count + 1 : count - 1;
    }

    if (type === 'down') {
        const toggledOn = downBtn.classList.toggle('challenge-detail__vote-btn--clicked-down');
        if (toggledOn && upBtn.classList.contains('challenge-detail__vote-btn--clicked-up')) {
            upBtn.classList.remove('challenge-detail__vote-btn--clicked-up');
            countSpan.textContent = count - 1;
        }
    }
}

const runButton = document.getElementById('challenge-detail-run');
const resultBody = document.getElementById('challenge-detail__result-body');
const tabs = document.querySelectorAll('.challenge-detail__tab');
const panels = document.querySelectorAll('.challenge-detail__tab-panel');

runButton.addEventListener('htmx:configRequest', function (e) {
    e.detail.parameters.sql = getEditorValue();
});

resultBody.addEventListener('htmx:afterSwap', applySyntaxMarkers);

tabs.forEach((tab) => {
    tab.addEventListener('click', () => {
        const target = tab.textContent.trim().toLowerCase();
        tabs.forEach((t) => t.classList.toggle('is-active', t === tab));
        panels.forEach((p) => p.classList.toggle('is-active', p.dataset.tab === target));
    });
});

runButton.addEventListener("htmx:beforeRequest", (event) => {
    clearSyntaxErrors();

    document.getElementById('challenge-detail-results-idle').classList.add('d-none');
    document.getElementById('challenge-detail-results-running').classList.remove('d-none');
    document.getElementById('challenge-detail__result-body').classList.add('d-none');

    const button = event.currentTarget;
    button.querySelector('#challenge-detail-run-text').textContent = 'Running...';
    button.disabled = true;
});

runButton.addEventListener("htmx:afterRequest", (event) => {
    document.getElementById('challenge-detail__result-body').classList.remove('d-none');
    document.getElementById('challenge-detail-results-running').classList.add('d-none');

    const button = event.currentTarget;
    button.disabled = false;
    button.querySelector('#challenge-detail-run-icon').classList.remove('d-none');
    button.querySelector('#challenge-detail-run-text').textContent = 'Run';
});

window.toggleVote = toggleVote;