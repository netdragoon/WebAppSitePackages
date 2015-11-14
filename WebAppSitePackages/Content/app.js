var app = angular.module("app", ['ngRoute', 'ui.router']);

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
          .otherwise({ redirectTo: '/' });
  }]);
app.controller('menu', function ($scope, $routeParams, $http) {



});
app.controller('cotacao', function ($scope, $routeParams, $http) {

    $scope.data = { 'Name': '' };
    $scope.clear = function()
    {
        $scope.data = { 'Name': '' };
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

})