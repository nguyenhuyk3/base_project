function createReviewCard(review) {
    // Tạo thẻ div mới
    console.log(review)
    var cardDiv = document.createElement("div");

    cardDiv.className = "card mb-2";

    // Tạo thẻ div cho nội dung của card
    var cardBodyDiv = document.createElement("div");

    cardBodyDiv.className = "card-body d-flex align-items-center";

    // Tạo thẻ div cho avatar
    var avatarDiv = document.createElement("div");

    avatarDiv.className = "avatar rounded-circle overflow-hidden mr-3";

    console.log(review)

    // Tạo thẻ hình ảnh
    var img = document.createElement("img");
    img.src = review.avatar; // Đặt src tùy thuộc vào đường dẫn hình ảnh thực tế
    img.alt = "Avatar";
    img.className = "img-fluid avatar-of-commenter";

    // Thêm hình ảnh vào thẻ avatar
    avatarDiv.appendChild(img);

    // Tạo thẻ div cho nội dung đánh giá
    var contentDiv = document.createElement("div");

    console.log(review)

    // Tạo tiêu đề của card
    var cardTitle = document.createElement("h5");
    cardTitle.className = "card-title";
    cardTitle.innerHTML = "<span>" + review.sender + "</span> đã đánh giá " + review.rating + " sao";

    // Tạo nội dung của card
    var cardText = document.createElement("p");
    cardText.className = "card-text";
    cardText.textContent = review.comment;

    // Thêm tiêu đề và nội dung vào thẻ content
    contentDiv.appendChild(cardTitle);
    contentDiv.appendChild(cardText);
    console.log(review)

    // Thêm các thẻ con vào thẻ cardBody
    cardBodyDiv.appendChild(avatarDiv);
    cardBodyDiv.appendChild(contentDiv);

    // Thêm thẻ cardBody vào thẻ card
    cardDiv.appendChild(cardBodyDiv);

    var reviewList = document.getElementById("review-list");
    var firstCard = reviewList.firstChild;
    console.log(review)
    if (firstCard) {
        reviewList.insertBefore(cardDiv, firstCard);
    } else {
        reviewList.appendChild(cardDiv); // Nếu danh sách rỗng, thêm vào cuối danh sách
    }
    console.log(review)
}

window.createReviewCard = createReviewCard;
