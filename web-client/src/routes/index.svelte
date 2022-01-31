<h1>Welcome to SvelteKit</h1>
<p>Visit <a href="https://kit.svelte.dev">kit.svelte.dev</a> to read the documentation</p>





<script> 
	// import Notifications from "svelte-notifications";
	import Chat from "./chat.svelte";
	import List from "./list.svelte";
	import Login from "./login.svelte";
	import Register from "./register.svelte"; 
  
	function handleLogin(event) {
		navigate("list", { replace: true });
	}
</script>
 
<!-- <Notifications> -->
    <div>
        <a href="login">login</a>
        <a href="register">register</a>
        <a href="list">list</a>
    </div> 
<!-- </Notifications> -->


