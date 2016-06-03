window.onload = function() {
    if (navigator.geolocation) {
        navigator.geolocation.getCurrentPosition(UserLocation)
    } else NearestCity(53.408371, -2.991573)
}

function UserLocation(position) {
    NearestCity(position.coords.latitude, position.coords.longitude)
}

function Deg2Rad(deg) {
    return deg * Math.PI / 180
}

function PythagorasEquirectangular(lat1, lon1, lat2, lon2) {
    lat1 = Deg2Rad(lat1);
    lat2 = Deg2Rad(lat2);
    lon1 = Deg2Rad(lon1);
    lon2 = Deg2Rad(lon2);
    var R = 6371;
    var x = (lon2 - lon1) * Math.cos((lat1 + lat2) / 2);
    var y = (lat2 - lat1);
    var d = Math.sqrt(x * x + y * y) * R;
    return d
}
var obj;
var getData = $.getJSON("/Umbraco/Api/Locations/GetLocations", function () {
    //console.log("success");
})
  .done(function (data) {
      var parsed = JSON.parse(data);
      obj = parsed;
      $.each(parsed, function (i, item) {          
          $("#city").append(item.LatLon);
      })
  })
  .fail(function () {
      console.log("error");
  })
  .always(function () {
      //console.log("complete");
  });

//other work
getData.complete(function () {
    //console.log("second complete");
});

function NearestCity(latitude, longitude) {
    var mindif = 99999;
    var closest;
    //latitude = 53.606766;
    //longitude = -0.585022;
    for (index = 0; index < obj.length; ++index) {
        var dif = PythagorasEquirectangular(latitude, longitude, obj[index].Latitude, obj[index].Longitude);
        if (dif < mindif) {
            closest = index;
            mindif = dif
        }
    }
    document.getElementById("city").innerHTML = "Tel:" + obj[closest].PhoneNumber;
}