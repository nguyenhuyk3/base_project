$(document).ready(function () {
    $('#confirmDeleteModal').on('shown.bs.modal', function (e) {
        var postId = $(e.relatedTarget).data('postid');

        $(this).data('postId', postId);
    });

    $('#confirmDelete').click(function () {
        var postId = $('#confirmDeleteModal').data('postId');

        console.log(postId);

        $.ajax({
            url: '/Customer/Post/DeletePost',
            type: 'POST',
            data: {
                postId: postId,
            },
            success: function (response) {
                console.log(response)

                if (response.success) {
                    window.location.reload();
                }
            },
            error: function () {

            }
        });
    });
});