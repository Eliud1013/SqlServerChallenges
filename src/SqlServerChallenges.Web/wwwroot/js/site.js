document.body.addEventListener('htmx:configRequest', function (event) {
    let tokenElement = document.querySelector('input[name="__RequestVerificationToken"]');
    
    if (tokenElement) {
        event.detail.headers['RequestVerificationToken'] = tokenElement.value;
    }
});