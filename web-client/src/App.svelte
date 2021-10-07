<script>
	import { Router, Link, Route } from "svelte-routing";
	import Notifications from "svelte-notifications";
	import Chat from "./routes/chat.svelte";
	import List from "./routes/list.svelte";
	import Login from "./routes/login.svelte";
	import Register from "./routes/register.svelte";
	import { navigate } from "svelte-routing";

	export let url = "login";

	function handleLogin(event) {
		navigate("list", { replace: true });
	}
</script>

<Notifications>
	<Router {url}>
		<div>
			<Route path="login">
				<Login on:successfulLogin={handleLogin} />
			</Route>
			<Route path="register">
				<Register/>
			</Route>
			<Route path="list">
				<List/>
			</Route>
		</div>
	</Router>
</Notifications>
