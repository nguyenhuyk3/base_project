"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();

connection.start().then(function () {
}).catch(function (err) {
    return console.error(err.toString());
});

document.addEventListener('DOMContentLoaded', function () {
    var sendResponseButton = document.getElementById('SendResponse');

    sendResponseButton.addEventListener('click', function (event) {
        event.preventDefault();

        var postId = document.getElementById('PostId').value;
        var xhr = new XMLHttpRequest();

        xhr.open('POST', '/Customer/Customer/CreateReponse?postId=' + postId, true);
        xhr.setRequestHeader('Content-Type', 'application/x-www-form-urlencoded');
        xhr.onreadystatechange = function () {
            if (xhr.readyState === XMLHttpRequest.DONE) {
                if (xhr.status === 200) {
                    var response = JSON.parse(xhr.responseText);
                    console.log(response)
                    if (response.success) {
                        var senderId = document.getElementById('SenderId').value;

                        console.log(senderId)
                    }
                }
            }
        }

        xhr.send();
    });
});