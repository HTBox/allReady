/// <binding Clean='clean' ProjectOpened='watch' />
var gulp = require("gulp"),
    rimraf = require("rimraf"),
    concat = require("gulp-concat"),
    cssmin = require("gulp-cssmin"),
    uglify = require("gulp-uglify"),
    debug = require("gulp-debug"),
    tsc = require('gulp-typescript'),
    tslint = require('gulp-tslint'),
    sourcemaps = require('gulp-sourcemaps'),
   // Config = require('./gulpfile.config'),
    project = require("./project.json");

//var config = new Config();

//https://www.npmjs.com/package/gulp-typescript
//http://weblogs.asp.net/dwahlin/creating-a-typescript-workflow-with-gulp
var tsProject = tsc.createProject('./tsconfig.json');

var paths = {
    webroot: "./" + project.webroot + "/"
};

paths.js = paths.webroot + "js/**/*.js";
paths.minJs = paths.webroot + "js/**/*.min.js";
paths.css = paths.webroot + "css/**/*.css";
paths.minCss = paths.webroot + "css/**/*.min.css";
paths.concatJsDest = paths.webroot + "js/site.min.js";
paths.concatCssDest = paths.webroot + "css/site.min.css";

gulp.task("clean:js", function (cb) {
    rimraf(paths.concatJsDest, cb);
});

gulp.task("clean:css", function (cb) {
    rimraf(paths.concatCssDest, cb);
});

gulp.task("clean", ["clean:js", "clean:css"]);


gulp.task("min:js", function () {
    return gulp.src([paths.js, "!" + paths.minJs], { base: "." })
        .pipe(concat(paths.concatJsDest))
        .pipe(uglify())
        .pipe(gulp.dest("."));
});


gulp.task("min:css", function () {
    return gulp.src([paths.css, "!" + paths.minCss])
        .pipe(concat(paths.concatCssDest))
        .pipe(cssmin())
        .pipe(gulp.dest("."));
});

gulp.task("min", ["min:js", "min:css"]);

gulp.task("watch", function () {
    gulp.watch(paths.css, ["min"]);
})


gulp.task('vender:JS', function () {
    var list = [
        'http://ecn.dev.virtualearth.net/mapcontrol/mapcontrol.ashx?v=7.0',
        'bower_components/jquery/dist/jquery.js',
        'bower_components/bootstrap/dist/js/bootstrap.js',
        'bower_components/bootstrap-datepicker/dist/js/bootstrap-datepicker.js',
        'bower_components/bootstrap-tagsinput/dist/bootstrap-tagsinput.js',
        'bower_components/knockout/dist/knockout.js',
        'bower_components/knockout-bootstrap/src/knockout-bootstrap.js',
        'bower_components/moment/moment.js',
        'bower_components/tota11y/build/tota11y.js',
        'bower_components/tinymce/tinymce.js',
        'bower_components/system.js/dist/system.js',
        'bower_components/system.js/dist/system-polyfills.js',
        paths.webroot + 'js/ko.filterableList.js',
        paths.webroot + 'js/ko.validation.js',
    ];

    gulp.src(list)
    .pipe(concat('vender.js'))
    .pipe(gulp.dest(paths.webroot + '/vendor'));
});

gulp.task('vender:CSSLayout', function () {
    var list = [
    'bower_components/bootstrap/dist/css/bootstrap.css',
    'bower_components/bootstrap-datepicker/dist/css/bootstrap-datepicker.css',
    'bower_components/bootstrap-toggle/css/bootstrap-toggle.css',
    'bower_components/font-awesome/css/font-awesome.css',
    'bower_components/bootstrap-tagsinput/src/bootstrap-tagsinput.css',
    'wwwroot/css/site.css',
    ];

    // place code for your default task here
    gulp.src(list)
    .pipe(concat('venderLayout.css'))
    .pipe(gulp.dest(paths.webroot + '/vendor'));

});

gulp.task('vender:CSSLayoutMainPage', function () {
    var list = [
    'bower_components/bootstrap/dist/css/bootstrap.css',
    'wwwroot/css/site.css',
    'wwwroot/css/mainPage.css',
    ];

    // place code for your default task here
    gulp.src(list)
    .pipe(concat('venderMainPage.css'))
    .pipe(gulp.dest(paths.webroot + '/vendor'));

});


//https://www.npmjs.com/package/gulp-typescript
gulp.task('typescript:Build', function () {

    var allTypeScript = 'ts/**/*.ts';
    var libraryTypeScriptDefinitions = './tools/typings/**/*.ts';
    var sourceTsFiles = [allTypeScript,                //path to typescript files
                         libraryTypeScriptDefinitions]; //reference to library .d.ts files


    return gulp.src(sourceTsFiles)
        .pipe(tsc({
            noImplicitAny: false,
            target: "es5",
            module: "system",
            declaration: true,
            noEmitOnError : false,
            moduleResolution: 'node',
            project: 'tsconfig.json',           
        }))
        .pipe(gulp.dest('wwwroot/js'));

});








/**
 * Lint all custom TypeScript files.
 */
//gulp.task('ts-lint', function () {
//    return gulp.src(config.allTypeScript).pipe(tslint()).pipe(tslint.report('prose'));
//});

/**
 * Compile TypeScript and include references to library and app .d.ts files.
 */
//gulp.task('compile-ts', function () {
//    var allTypeScript = 'ts/**/*.ts';
//    var libraryTypeScriptDefinitions = './tools/typings/**/*.ts';
//    var sourceTsFiles = [allTypeScript,                //path to typescript files
//                         libraryTypeScriptDefinitions]; //reference to library .d.ts files


//    var tsResult = gulp.src(sourceTsFiles)
//                       .pipe(sourcemaps.init())
//                       .pipe(tsc(tsProject));

//    tsResult.dts.pipe(gulp.dest(config.tsOutputPath));
//    return tsResult.js
//                    .pipe(sourcemaps.write('.'))
//                    .pipe(gulp.dest(config.tsOutputPath));
//});

/**
 * Remove all generated JavaScript files from TypeScript compilation.
 */
//gulp.task('clean-ts', function (cb) {
//    var typeScriptGenFiles = [
//                                config.tsOutputPath + '/**/*.js',    // path to all JS files auto gen'd by editor
//                                config.tsOutputPath + '/**/*.js.map', // path to all sourcemap files auto gen'd by editor
//                                '!' + config.tsOutputPath + '/lib'
//    ];

//    // delete the files
//    del(typeScriptGenFiles, cb);
//});

