import Vue from 'vue';
import Router from 'vue-router';
import Home from './views/Home.vue';
import AboutAllready from './components/AboutAllready.vue';
import AesopsTale from './components/AesopsTale.vue';

Vue.use(Router);

export default new Router({
  mode: 'history',
  base: process.env.BASE_URL,
  routes: [
    {
      path: '/',
      name: 'home',
      component: Home,
    },
    {
        path: '/about/',
        name: 'about',
        component: () => import(/* webpackChunkName: "about" */ './views/About.vue'),
        children: [
            {
                path: '',
                component: AboutAllready
            },
            {
                path: 'allReady',
                component: AboutAllready
            },
            {
                path: 'aesop',
                component: AesopsTale
            }
          ]
      },
    ],
});
