<template>
  <div id="app">
    <el-container>
      <el-header>
        <el-menu class="el-menu-demo" mode="horizontal" default-active="/" :router="true">
          <el-menu-item index="/" :route="{ name: 'Home' }">Home</el-menu-item>
          <el-menu-item index="/users" :route="{ name: 'Users' }"
            >Users</el-menu-item
          >
          <el-menu-item index="/reports" :route="{ name: 'Reports' }"
            >Reports</el-menu-item
          >
        </el-menu>
      </el-header>
      
      <el-main>
        <router-view />
      </el-main>
    </el-container>
  </div>
</template>

<style lang="scss">
</style>
