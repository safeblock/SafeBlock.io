$(document).ready(function () {
    NProgress.start();
    NProgress.done();

    $(function () {
        $('[data-toggle="tooltip"]').tooltip()
    })

    $(".avatar").hover(function (e) {
        $(this).find(".overlay").removeClass("hide");
    }, function () {
        $(this).find(".overlay").addClass("hide");
    });
});