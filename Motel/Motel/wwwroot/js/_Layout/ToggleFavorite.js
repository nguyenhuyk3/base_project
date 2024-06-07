// This function will truncate text if it have more than 'maxLength'
function truncateText(text, maxLength) {
    if (text.length > maxLength) {
        return text.slice(0, maxLength) + "...";
    } else {
        return text;
    }
}

function setFavoriteCounter(success) {
    var favoritesCounter = document.getElementById('favoritesCounter');
    var numberOfFavorites = parseInt(favoritesCounter.textContent);

    if (success) {
        numberOfFavorites++;
    } else {
        numberOfFavorites--;
    }

    favoritesCounter.textContent = numberOfFavorites.toString();
}

// Set status of favorite button on 'local storage'
function toggleFavoriteStatus(postId, button) {
    var userId = button.closest(".favorite-form").getAttribute("data-userid");
    var key = userId + "_" + postId;
    var isLiked = localStorage.getItem(key) === "true";

    isLiked = !isLiked;
    localStorage.setItem(key, isLiked.toString());
    button.classList.toggle("active", isLiked);

    return isLiked;
}

document.addEventListener("DOMContentLoaded", function () {
    var favoriteButtons = document.querySelectorAll(".favorite-btn");

    // When loading the page this function will check all post to see it is liked yet 
    favoriteButtons.forEach(function (button) {
        var postId = button.closest(".favorite-form").getAttribute("data-postid");
        var userId = button.closest(".favorite-form").getAttribute("data-userid");
        var key = userId + "_" + postId;
        var isLiked = localStorage.getItem(key) === "true";

        button.classList.toggle("active", isLiked);
    });

    // When clicking 'favorite-btn' it will like this post
    document.querySelectorAll(".favorite-btn").forEach(function (button) {
        button.addEventListener("click", function () {
            var postId = button.closest(".favorite-form").getAttribute("data-postid");
            var isLiked = toggleFavoriteStatus(postId, button);
            var xhr = new XMLHttpRequest();

            xhr.open("POST", isLiked ? "/Post/Home/AddToFavorites" : "/Post/Home/RemoveFromFavorites", true);
            xhr.setRequestHeader("Content-Type", "application/x-www-form-urlencoded");
            xhr.onload = function () {
                if (xhr.status === 200) {
                    var response = JSON.parse(xhr.responseText);

                    if (response.success) {
                        if (isLiked) {
                            var favoritePost = response.favoritePost;

                            // This code below will functionalize later 
                            // ========== || ==========
                            // <div class="favorite-item" data-postid="${favoritePost.postId}">
                            var newFavoriteItem = document.createElement("div");

                            newFavoriteItem.classList.add("favorite-item");
                            newFavoriteItem.setAttribute("data-postid", favoritePost.postId);

                            // <div class="d-flex align-items-center">
                            var contentContainer = document.createElement("div");

                            contentContainer.classList.add("d-flex", "align-items-center");

                            // <img src="${favoritePost.img}" alt="Ảnh phòng trọ">
                            var imgElement = document.createElement("img");

                            imgElement.src = favoritePost.img;
                            imgElement.alt = "Ảnh phòng trọ";

                            // <div>
                            var textContainer = document.createElement("div");
                            // <h6 class="text-truncate-custom">${favoritePost.subjectOnSite}</h6>
                            var h6Element = document.createElement("h6");

                            h6Element.textContent = truncateText(favoritePost.subjectOnSite, 38);

                            // <p>${favoritePost.createdAt}</p>
                            var pElement = document.createElement("p")
                            var createdAt = new Date(favoritePost.createdAt);
                            var formattedDate = createdAt.toLocaleString('vi-VN', {
                                year: 'numeric',
                                month: '2-digit',
                                day: '2-digit',
                                hour: '2-digit',
                                minute: '2-digit',
                            });

                            pElement.textContent = formattedDate

                            contentContainer.appendChild(imgElement);
                            contentContainer.appendChild(textContainer);
                            textContainer.appendChild(h6Element);
                            textContainer.appendChild(pElement);

                            var removeButton = document.createElement("span");

                            // <span class="remove-favorite">&times;</span>
                            removeButton.classList.add("remove-favorite");
                            removeButton.textContent = "×";

                            newFavoriteItem.appendChild(contentContainer);
                            newFavoriteItem.appendChild(removeButton);

                            var dropdownMenu = document.querySelector(".dropdown-menu-favorites");

                            dropdownMenu.insertBefore(newFavoriteItem, dropdownMenu.firstChild);
                            // ========== || ==========

                            // Set counter when favorite post is added
                            setFavoriteCounter(response.success);

                            // Adding 'click event' to 'removeButton' when it is created
                            removeButton.addEventListener('click', function () {
                                var favoriteItem = removeButton.closest('.favorite-item');
                                var userId = document.getElementById('ownerId').value;
                                var postId = favoriteItem.getAttribute('data-postid');

                                favoriteItem.parentNode.removeChild(favoriteItem);

                                var xhr = new XMLHttpRequest();

                                xhr.open('POST', '/Post/Home/RemoveFromFavorites', true);
                                xhr.setRequestHeader("Content-Type", "application/x-www-form-urlencoded");
                                xhr.onload = function () {
                                    if (xhr.status === 200) {
                                        var response = JSON.parse(xhr.responseText);

                                        if (response.success == false) {
                                            var favoriteForm = document.querySelector('form[data-postid="' + postId + '"]');
                                            var favoriteButton = favoriteForm.querySelector('.favorite-btn');
                                            var isLiked = updateTogglingFavoriteStatus(userId, postId);

                                            favoriteButton.classList.toggle("active", isLiked);
                                        }
                                    }
                                }

                                xhr.send("postId=" + encodeURIComponent(postId));

                                updateFavorites();
                            });
                        }
                    } else {
                        var favoriteItem = document.querySelector('.favorite-item[data-postid="' + postId + '"]');


                        if (favoriteItem) {

                            favoriteItem.parentNode.removeChild(favoriteItem);
                        }

                        // Set counter when favorite post is minus
                        setFavoriteCounter(response.success);


                    }

                    updateFavorites();
                }
            };

            xhr.send("postId=" + encodeURIComponent(postId));
        });
    });
});