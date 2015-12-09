var app = angular.module("app", ['ngRoute', 'ui.router','youtube-embed']);

//app.run(function (amMoment) {
//    amMoment.changeLocale('pt-BR');
//});

app.directive('onlyNum', function () {
    return function (scope, element, attrs) {
        var keyCode = [8, 9, 13, 37, 39, 48, 49, 50, 51, 52, 53, 54, 55, 56, 57, 96, 97, 98, 99, 100, 101, 102, 103, 104, 105, 110];
        element.bind("keydown", function (event) {            
            if ($.inArray(event.which, keyCode) == -1) {
                scope.$apply(function () {
                    scope.$eval(attrs.onlyNum);
                    event.preventDefault();
                });
                event.preventDefault();
            }
        });
    };
});

app.config(['$routeProvider',
  function ($routeProvider) {
      $routeProvider
          .when('/', {
              templateUrl: '/view/home',
              controller: 'menu'
          })
          .when('/zipcode', {
              templateUrl: '/view/zipcode',
              controller: 'zipcode'
          })
          .when('/cotacao', {
              templateUrl: '/view/cotacao',
              controller: 'cotacao'
          })
          .when('/address', {
              templateUrl: '/view/address',
              controller: 'address'
          })
          .when('/forecast', {
              templateUrl: '/view/forecast',
              controller: 'forecast'
          })
          .when('/gravatar', {
              templateUrl: '/view/gravatar',
              controller: 'gravatar'
          })
          .when('/shorturl', {
              templateUrl: '/view/shorturl',
              controller: 'shorturl'
          })
          .when('/thumbnail', {
              templateUrl: '/view/thumbnail',
              controller: 'thumbnail'
          })
          .otherwise({ redirectTo: '/' });
  }]);
app.controller('menu', function ($scope, $routeParams, $http)
{
    $scope.load = function()
    {
        ancoraSet("up");
    }
    $scope.load();
});
app.controller('cotacao', function ($scope, $routeParams, $http) {

    $scope.data = { 'Name': '' };
    $scope.clear = function()
    {
        $scope.data = { 'Name': '' };
        ancoraSet("up");
    }
    $scope.submit = function()
    {
        $scope.clear();
        $http.post('/operation/cotacao', { })
                .success(function (data) {
                    data.dateBR = moment(data.Date).format('DD/MM/YYYY');
                    $scope.data = data;                    

                })
                .error(function (data) {
                    $scope.clear();
                });
    }
    $scope.submit();

});
app.controller('address', function ($scope, $routeParams, $http) {
    $scope.uf = 'SP';
    $scope.city = '';
    $scope.address = '';
    $scope.data = [];
    $scope.message = '';
    $scope.loading = false;

    $scope.clear = function () {
        $scope.uf = 'SP';
        $scope.city = '';
        $scope.address = '';
        $scope.data = [];
        $scope.message = '';
        $scope.loading = false;
        $("#uf").focus();
        ancoraSet("up");
    }

    $scope.submit = function ()
    {
        if ($scope.city.length > 2 && $scope.address.length > 2) {
            $scope.message = '';
            $scope.loading = true;
            $scope.data = [];
            $http.post('/operation/address', { 'uf': $scope.uf, 'city': $scope.city, 'address': $scope.address })
                    .success(function (data) {
                        if (parseInt(data.length) === 0) {
                            $scope.message = 'Nenhum item encontrado';
                        }
                        $scope.data = data;
                        $scope.loading = false;
                    })
                    .error(function (data) {
                        $scope.clear();
                    });
        } else {
            $scope.message = 'Cidade e endereço é obrigatório mais de 2 letras';
        }
    }

});
app.controller('zipcode', function ($scope, $routeParams, $http) {

    $scope.cep = '';
    $scope.data = {};
    $scope.message = '';
    $scope.loading = false;

    $scope.clear = function ()
    {
        $scope.cep = '';
        $scope.data = { 'Zip': "", 'Address': "", 'District': "", 'City': "", 'Uf': "", 'Ibge': "", 'Erro': true, 'Complement': "", 'Gia': "" };
        $scope.message = '';
        $scope.loading = false;
        $("#cep").focus();
        ancoraSet("up");
    }

    $scope.submit = function ()
    {
        if ($scope.verify()) {
            $scope.loading = true;
            $http.post('/operation/zipcode', { 'cep': $scope.cep })
                .success(function (data) {                    
                    if (data.Erro) {
                        $scope.clear();
                        $scope.message = 'CEP inválido ...';
                    } else {
                        $scope.data = data;
                        $scope.message = '';
                    }
                    $scope.loading = false;
                })
                .error(function (data) {
                    $scope.clear();                    
                });
        } else {
            $scope.clear();
            $scope.message = 'CEP inválido ...';
        }
        
    }

    $scope.verify = function () {
        var tstCep = /^[0-9]{8}$/;
        return $scope.cep !== '' && $scope.cep.length === 8 && tstCep.test($scope.cep);
    }

    $scope.clear();

});
app.controller('forecast', function ($scope, $routeParams, $http)
{
    $scope.loading = false;
    $scope.name = '';
    $scope.cities = [];
    $scope.prevision = {};
    $scope.message = '';

    $scope.clear = function () {
        $scope.loading = false;
        $scope.name = '';
        $scope.cities = [];
        $scope.prevision = {};
        $scope.message = '';
        ancoraSet("up");
    }

    $scope.submit = function () {
        if ($scope.name !== '' && $scope.name.length > 2) {
            $scope.loading = true;
            $http.post('/operation/cities', { 'name': $scope.name })
                .success(function (data) {
                    $scope.cities = data.Citys;
                    $scope.loading = false;
                    if (parseInt(data.Count) === 0)
                    {
                        $scope.message = 'Cidade inexistente ...';
                    }
                })
                .error(function (data) {
                    $scope.clear();
                });
        }
        else
        {
            alert('Digite a cidade com no minimo 3 letras')    
        }
    }

    $scope.load = function(Id)
    {
        $scope.loading = true;
        $http.post('/operation/prevision', { 'Id': Id, 'Quant': 4 })
            .success(function (data) {                
                $scope.prevision = data;
                $scope.prevision.Id = Id;
                $scope.prevision.UpdatedBR =
                    moment(data.Update).format('DD/MM/YYYY');
                for (i = 0; i < $scope.prevision.Days.length; i++)
                {
                    $scope.prevision.Days[i].DataBR =
                        moment($scope.prevision.Days[i].Data)
                            .format('DD/MM/YYYY');
                }
                $scope.loading = false;
            })
            .error(function (data) {
                $scope.clear();
            });
    }

});
app.controller('gravatar', function ($scope, $routeParams, $http)
{
    $scope.loading = false;
    $scope.data = {};    
    $scope.message = '';
    $scope.email = '';
    $scope.width = '100';

    $scope.clear = function () {
        $scope.width = '100';
        $scope.loading = false;
        $scope.data = {};
        $scope.message = '';
        $scope.email = '';
        ancoraSet("up");
    }

    $scope.submit = function () {
        if ($scope.email !== '') {
            $scope.loading = true;
            $http.post('/operation/gravatar', { 'email': $scope.email, 'width': parseInt($scope.width) })
                .success(function (data) {
                    if (data.error == false)
                    {
                        $scope.data = data.item;
                        $scope.data.erro = false;
                        $scope.message = '';
                    }
                    else
                    {
                        $scope.message = 'E-mail não encontrado';
                    }
                    $scope.loading = false;
                })
                .error(function (data) {
                    $scope.clear();
                });
        }
        else {
            alert('Digite o e-mail não existe')
        }
    }
    $scope.clear();
});
app.controller('shorturl', function ($scope, $routeParams, $http) {

    $scope.loading = false;
    $scope.data = {};
    $scope.message = '';
    $scope.url = '';

    //{"ShortUrl":"https://tr.im/elGg5","Keyword":"elGg5","Url":"http://www.uol.com.br/"}

    $scope.clear = function ()
    {        
        $scope.loading = false;
        $scope.data = {};
        $scope.message = '';
        $scope.url = '';
        ancoraSet("up");
    }

    $scope.submit = function () 
    {
        if ($scope.url !== '' && is_valid_url($scope.url))
        {
            $scope.loading = true;
            $http.post('/operation/shorturl', { 'Url': $scope.url })
                .success(function (data) {
                    if (data.error == false) {
                        var d = data.data;
                        $scope.data.shorturl = d.ShortUrl;
                        $scope.data.Keyword = d.Keyword;
                        $scope.data.url = d.Url;
                        $scope.data.erro = false;
                        $scope.message = '';
                    }
                    else {
                        $scope.message = 'Url inválida';
                    }
                    $scope.loading = false;
                })
                .error(function (data) {
                    $scope.clear();
                });
        }
        else
        {
            alert('Digite uma url válida');
        }
    }
    $scope.clear();
});
app.controller('thumbnail', function ($scope, $routeParams, $http) {

    $scope.loading = false;
    $scope.data = {};
    $scope.message = '';
    $scope.url = '';    

    $scope.clear = function () {
        $scope.loading = false;
        $scope.data = {};
        $scope.message = '';
        $scope.url = '';
        ancoraSet("up");
    }
    /*
    {"data":{"pic0":"/thumb/Grdz-5QR2Tk-Picture0.jpg"
        ,"pic1":"/thumb/Grdz-5QR2Tk-Picture1.jpg"
        ,"pic2":"/thumb/Grdz-5QR2Tk-Picture2.jpg"
        ,"pic3":"/thumb/Grdz-5QR2Tk-Picture3.jpg"
        ,"def":"/thumb/Grdz-5QR2Tk-Default.jpg"

        ,"hg":"/thumb/Grdz-5QR2Tk-HightQuality.jpg"
        ,"mx":"/thumb/Grdz-5QR2Tk-MaxResolution.jpg"
        ,"mq":"/thumb/Grdz-5QR2Tk-MediumQuality.jpg"
        ,"st":"/thumb/Grdz-5QR2Tk-Standard.jpg"
        ,"embed":"\u003ciframe width=\"560\" height=\"315\" src=\"https://www.youtube.com/embed/Grdz-5QR2Tk\" frameborder=\"0\" allowfullscreen\u003e\u003c/iframe\u003e"
        ,"share":"https://youtu.be/Grdz-5QR2Tk"},"error":false}
        */
    $scope.submit = function () {
        if ($scope.url !== '' && is_valid_url($scope.url)) {
            $scope.loading = true;
            $http.post('/operation/thumbnail', { 'Url': $scope.url })
                .success(function (data) {
                    if (data.error == false) {
                        var d = data.data;
                        $scope.data = d;                        
                        $scope.data.erro = false;
                        $scope.message = '';                        
                    }
                    else
                    {
                        $scope.clear();
                        $scope.message = 'Url inválida';
                    }
                    $scope.loading = false;
                })
                .error(function (data) {
                    $scope.clear();
                });
        }
        else {
            alert('Digite uma url válida');
        }
    }
    $scope.clear();
});