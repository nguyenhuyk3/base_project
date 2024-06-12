// Get key on local storage to check favorite status
function updateTogglingFavoriteStatus(userId, postId) {
    var key = userId + "_" + postId;
    var isLiked = localStorage.getItem(key) === "true";

    isLiked = !isLiked;
    localStorage.setItem(key, isLiked.toString());

    return isLiked;
}

// Get all favorite posts when loading the page
document.addEventListener("DOMContentLoaded", function () {
    var receiverEmail = document.getElementById("receiverEmail").value;

    if (receiverEmail.length !== 0) {
        var xhr = new XMLHttpRequest();

        xhr.open('GET', 'Post/Home/GetAllFavoritePosts', true);
        xhr.setRequestHeader("Content-Type", "application/json");

        xhr.onload = function () {
            if (xhr.readyState === 4) {
                if (xhr.status === 200) {
                    var response = JSON.parse(xhr.responseText);

                    if (response.success) {
                        var favoritePosts = response.favoritePosts;

                        favoritePosts.forEach(function (favoritePost) {
                            var newFavoriteItem = document.createElement("div");

                            newFavoriteItem.classList.add("favorite-item");
                            newFavoriteItem.setAttribute("data-postid", favoritePost.postId);

                            var contentContainer = document.createElement("div");

                            contentContainer.classList.add("d-flex", "align-items-center");

                            var imgElement = document.createElement("img");

                            imgElement.src = favoritePost.img;
                            imgElement.alt = "Ảnh phòng trọ";

                            var textContainer = document.createElement("div");
                            var h6Element = document.createElement("h6");

                            h6Element.textContent = truncateText(favoritePost.subjectOnSite, 38);

                            var pElement = document.createElement("p");
                            var createdAt = new Date(favoritePost.createdAt);
                            var formattedDate = createdAt.toLocaleString('vi-VN', {
                                year: 'numeric',
                                month: '2-digit',
                                day: '2-digit',
                                hour: '2-digit',
                                minute: '2-digit',
                            });

                            pElement.textContent = formattedDate;

                            contentContainer.appendChild(imgElement);
                            contentContainer.appendChild(textContainer);
                            textContainer.appendChild(h6Element);
                            textContainer.appendChild(pElement);

                            var removeButton = document.createElement("span");

                            removeButton.classList.add("remove-favorite");
                            removeButton.textContent = "×";
                            newFavoriteItem.appendChild(contentContainer);
                            newFavoriteItem.appendChild(removeButton);

                            var dropdownMenu = document.querySelector(".dropdown-menu-favorites");

                            dropdownMenu.insertBefore(newFavoriteItem, dropdownMenu.firstChild);

                            var favoritesCounter = document.getElementById('favoritesCounter');

                            favoritesCounter.textContent = response.count

                            // When clicking to remove favorite post on heart shape then it will perfom this function
                            removeButton.addEventListener('click', function () {
                                var favoriteItem = removeButton.closest('.favorite-item');
                                var userId = document.getElementById('OwnerId').value;
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
                        });

                        updateFavorites();
                    }
                }
            }
        }

        xhr.send();
    }
});