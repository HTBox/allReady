<template>
  <v-app id="inspire">
    <v-navigation-drawer v-model="showDrawer" fixed app>
      <v-list dense>
        <v-list-tile to="/">
          <v-list-tile-action>
            <v-icon>home</v-icon>
          </v-list-tile-action>
          <v-list-tile-content>
            <v-list-tile-title>Home</v-list-tile-title>
          </v-list-tile-content>
        </v-list-tile>
        <v-list-tile to="/about">
          <v-list-tile-action>
            <v-icon>contact_mail</v-icon>
          </v-list-tile-action>
          <v-list-tile-content>
            <v-list-tile-title>About</v-list-tile-title>
          </v-list-tile-content>
        </v-list-tile>
      </v-list>
    </v-navigation-drawer>
    <v-toolbar color="primary" dark fixed app>
      <v-toolbar-side-icon @click.stop="showDrawer = !showDrawer"></v-toolbar-side-icon>
      <v-toolbar-title>
        <router-link to="/">
          <img src="./assets/Web-Logo-small.png">
        </router-link>
      </v-toolbar-title>
    </v-toolbar>
    <v-content>
      <v-container fluid fill-height>
        <v-layout justify-center align-center>
          <transition name="fade" mode="out-in">
            <router-view/>
          </transition>
        </v-layout>
      </v-container>
    </v-content>
    <v-footer app>
      <v-layout justify-center align-left>
        <a class="footer-icon" href="http://www.htbox.org/blog/" target="_blank">
          <img alt="Check out the Humanitarian Toolbox blog" src="./assets/blog-small.png">
        </a>
        <a class="footer-icon" href="https://www.facebook.com/allReadyApp/" target="_blank">
          <img alt="Go to allReady on Facebook" src="./assets/fb-small.png">
        </a>
        <a class="footer-icon" href="https://twitter.com/htbox" target="_blank">
          <img alt="Follow Humanitarian Toolbox on Twitter" src="./assets/twitter-small.png">
        </a>
        <a
          class="footer-icon"
          href="https://www.youtube.com/watch?v=BgV9jUJduwE&amp;index=3&amp;list=PLReL099Y5nRfJCkJtGFHZlVCvx2t8OWdA"
          target="_blank"
        >
          <img alt="YouTube" src="./assets/youtube-small.png">
        </a>
        <v-spacer></v-spacer>
        <div>
          Powered by
          <a href="http://www.htbox.org/" target="_blank">Humanitarian Toolbox</a>
        </div>
      </v-layout>
    </v-footer>
  </v-app>
</template>

<script>
export default {
  name: "App",
  components: {},
  data() {
    return {
      showDrawer: false
      //
    };
  }
};
</script>
<style>
.footer-icon {
  margin: 5px;
}

.fade-enter-active,
.fade-leave-active {
  transition-duration: 0.3s;
  transition-property: opacity;
  transition-timing-function: ease;
}

.fade-enter,
.fade-leave-active {
  opacity: 0;
}
</style>

