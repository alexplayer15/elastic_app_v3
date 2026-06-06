import SignUpForm from '../components/SignUpForm';
import {paths} from "./paths";
import LoginForm from "../components/LoginForm";
import HomeForm from "../components/HomeForm";

export const routes = [
    { path: paths.signUp, element: SignUpForm },
    { path: paths.login, element: LoginForm },
    { path: paths.home, element: HomeForm }
];