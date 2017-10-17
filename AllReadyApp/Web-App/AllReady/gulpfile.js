/// <binding Clean='clean' ProjectOpened='watch' />
var gulp = require("gulp"),
    rimraf = require("rimraf"),
    concat = require("gulp-concat"),
    cssmin = require("gulp-cssmin"),
    ts = require('gulp-typescript'),
    fs = require('fs');

// https://www.npmjs.com/package/gulp-typescript
// http://weblogs.asp.net/dwahlin/creating-a-typescript-workflow-with-gulp
var tsProject = ts.createProject('tsconfig.json');

var paths = {
    webroot: "./wwwroot/"
};

paths.ts = paths.webroot + "{ts,js}/**/*.ts";
paths.css = paths.webroot + "css/**/*.css";
paths.minCss = paths.webroot + "css/**/*.min.css";
paths.concatCssDest = paths.webroot + "css/site.min.css";

gulp.task("clean", function (cb) {
    rimraf(paths.concatCssDest, cb);
});

gulp.task('build:lib', function () {
    function getNPMDependencies() {
        var buffer, packages, keys;
        buffer = fs.readFileSync('package.json');
        packages = JSON.parse(buffer.toString());
        keys = [];
    
        for (var key in packages.dependencies) {
            keys.push(key);
        }
    
        return keys;
    }

    function copyNodeModule(name) {
        gulp.src('node_modules/' + name + '/**/*')
            .pipe(gulp.dest('wwwroot/lib/' + name + '/'));
    };

    var dependencies = getNPMDependencies();
    for (var i in dependencies) {
        copyNodeModule(dependencies[i])
    }
})

gulp.task('build:ts', function () {
    var tsResult = tsProject.src()
        .pipe(tsProject())
        .js.pipe(gulp.dest(paths.webroot));
});

gulp.task("build:css", function () {
    return gulp.src([paths.css, "!" + paths.minCss])
        .pipe(concat(paths.concatCssDest))
        .pipe(cssmin())
        .pipe(gulp.dest("."));
});

gulp.task("build", ["build:lib", "build:ts", "build:css"]);
gulp.task("min", ["build"]);

gulp.task("watch", function () {
    gulp.watch([paths.css, paths.ts], ["build"]);
});

