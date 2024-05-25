//$(document).ready(function () {
//    let articlesPerPage = 4;
//    let totalArticles = $(".article").length;
//    $(".article").slice(articlesPerPage).addClass("hidden");

//    $("#loadMore").on("click", function (e) {
//        e.preventDefault();
//        let visibleArticles = $(".article:not(.hidden)").length;
//        $(".article.hidden").slice(0, articlesPerPage).removeClass("hidden");

//        visibleArticles = $(".article:not(.hidden)").length; // Cập nhật lại số lượng bài viết hiển thị


//        if (visibleArticles >= totalArticles) {
//            $("#loadMoreButtonContainer").addClass("hidden");
//        }
//    });
//});

document.addEventListener("DOMContentLoaded", function () {
    var articlesPerPage = 4;
    var totalArticles = document.querySelectorAll(".article").length;
    var loadMoreButton = document.getElementById("loadMore");

    var loadMoreClickHandler = function (e) {
        e.preventDefault();
        var visibleArticles = document.querySelectorAll(".article:not(.hidden)").length;
        var hiddenArticles = document.querySelectorAll(".article.hidden");

        for (var i = 0; i < articlesPerPage; i++) {
            if (hiddenArticles[i]) {
                hiddenArticles[i].classList.remove("hidden");
            }
        }

        visibleArticles = document.querySelectorAll(".article:not(.hidden)").length;

        if (visibleArticles >= totalArticles) {
            loadMoreButton.parentNode.classList.add("hidden");
        }
    };

    loadMoreButton.addEventListener("click", loadMoreClickHandler);
});