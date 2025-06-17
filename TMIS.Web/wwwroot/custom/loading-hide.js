
$(window).on('load', function () {
  $(".loader").fadeOut(200, function () {
    $(".panelDiv").css("opacity", 0).fadeIn(200).css("opacity", 1);
  });
});
