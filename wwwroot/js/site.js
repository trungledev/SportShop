// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
var chatbox = document.getElementById('fb-customer-chat');
chatbox.setAttribute("page_id", "113024225059852");
chatbox.setAttribute("attribution", "biz_inbox");
//  Your SDK code 
window.fbAsyncInit = function () {
    FB.init({
        xfbml: true,
        version: 'v16.0'
    });
};

(function (d, s, id) {
    var js, fjs = d.getElementsByTagName(s)[0];
    if (d.getElementById(id)) return;
    js = d.createElement(s); js.id = id;
    js.src = 'https://connect.facebook.net/vi_VN/sdk/xfbml.customerchat.js';
    fjs.parentNode.insertBefore(js, fjs);
}(document, 'script', 'facebook-jssdk'));

$("ul#categories").load("/Home/CategoriesLayout");
$("#btnBackToTop").click(function (event) {
    event.preventDefault();
    $("html, body").animate({ scrollTop: 0 }, "slow");
});
$('input.rating').rating({
    filled: 'bi bi-star-fill',
    empty: 'bi bi-star'
});


