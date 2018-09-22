var is_down = false;

toastr.options = {
  "closeButton": false,
  "debug": false,
  "newestOnTop": true,
  "progressBar": true,
  "positionClass": "toast-bottom-left",
  "preventDuplicates": true,
  "onclick": null,
  "showDuration": "300",
  "hideDuration": "1000",
  "timeOut": "5000",
  "extendedTimeOut": "1000",
  "showEasing": "swing",
  "hideEasing": "linear",
  "showMethod": "fadeIn",
  "hideMethod": "fadeOut"
}

$(document).ready(function () {
    NProgress.start();
    NProgress.done();
    $('.open-popup-link').magnificPopup({
        type: 'inline',
        midClick: true // allow opening popup on middle mouse click. Always set it to true if you don't provide alternative source.
    });
    
    $(function () {
        $('[data-toggle="tooltip"]').tooltip()
    })

    var scroll = $(window).scrollTop();
    if(scroll >= 685)
    {
        $("#navbar").addClass("active");
        $("#navbar").css("position", "fixed");
        $(".logo").attr("src", "/static/logo-black.png");
        is_down = true;
    }

    var items = document.querySelectorAll(".accordion a");

    function toggleAccordion() {
        this.classList.toggle('active');
        this.nextElementSibling.classList.toggle('active');
    }

    items.forEach(item => item.addEventListener('click', toggleAccordion));
});

$(window).scroll(function (event)
{
    var scroll = $(window).scrollTop();
    if(scroll >= 300)
    {
        if(!is_down)
        {
            $("#navbar").addClass("active");
            $("#navbar .container-fluid").removeClass("py-4").addClass("py-3");
            $(".logo").attr("src", "/static/logo-black.png");
            is_down = true;
        }
    }
    else if (scroll <= 0)
    {
        if(is_down)
        {
            $("#navbar .container-fluid").removeClass("py-3").addClass("py-4");
            $("#navbar").removeClass("active");
            $(".logo").attr("src", "/static/logo.png");
            is_down = false;
        }
    }
});

function SendedMessage()
{
    toastr.success('Thanks, you will receive a reply within a few days. 🙂');
}

function FailedMessage()
{
    toastr.error('Arf, we have a problem with the support :( Try with Twitter');
}

function SuccessfullySubscribed()
{
    $("#email-button-newsletter").html("<i class=\"fas fa-check\"></i> Suscribed !");
    $("#email-input-newsletter").prop('disabled', true);
    $("#email-button-newsletter").addClass("btn-success").addClass("disabled");
    toastr.success('Thank you, you will informed every weeks. 📧')
}

function FailedSubscribed()
{
    toastr.error('Arf, we are currently unable to add you to our subscriber list.')
}