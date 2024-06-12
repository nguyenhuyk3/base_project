document.addEventListener('DOMContentLoaded', function () {
    var createAuthenticatingButton = document.getElementById('CreateAuthenticating');

    createAuthenticatingButton.addEventListener('click', function (event) {
        event.preventDefault();

        var postId = document.getElementById('PostId').value;
        var xhr = new XMLHttpRequest();

        xhr.open('POST', '/Admin/Post/CreateAuthenticating?postId=' + postId, true);
        xhr.setRequestHeader('Content-Type', 'application/x-www-form-urlencoded');
        xhr.onreadystatechange = function () {
            if (xhr.readyState === XMLHttpRequest.DONE) {
                if (xhr.status === 200) {
                    var response = JSON.parse(xhr.responseText);
                    if (response.success) {
                        createAuthenticatingButton.classList.remove('btn-outline-success');
                        createAuthenticatingButton.classList.add('btn-success');
                        createAuthenticatingButton.textContent = 'Tin đã xác thực';
                        createAuthenticatingButton.disabled = true;
                    }
                }
            }
        };

        xhr.send();
    });
});