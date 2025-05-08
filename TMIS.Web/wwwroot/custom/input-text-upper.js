document.addEventListener("input", function (event) {
  if (event.target.tagName === "INPUT" || event.target.tagName === "TEXTAREA") {
    event.target.value = event.target.value.toUpperCase();
  }
});
