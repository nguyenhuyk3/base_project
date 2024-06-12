$(document).ready(function () {
    $.ajax({
        url: '@Url.Action("GetTerm", "Home", new { area = "Post" })',
        method: 'GET',
        success: function (data) {
            let subjects = data.filter(function (item) {
                return item !== undefined && item !== null;
            });

            $("#searchInput").autocomplete({
                source: subjects,
                minLength: 1
            });
        },
        error: function (xhr, status, error) {
            console.error("Error fetching subjects: " + error);
        }
    });

    $.ajax({
        url: '@Url.Action("GetTerm", "Home", new { area = "Post" })',
        method: 'GET',
        success: function (data) {
            $("#searchInput").autocomplete({
                source: data,
                minLength: 1
            });
        },
        error: function (xhr, status, error) {
            console.error("Error fetching subjects: " + error);
        }
    });
});