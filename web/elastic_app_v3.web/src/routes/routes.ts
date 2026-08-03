import SignUpForm from '../components/SignUpForm';
import {paths} from "./paths";
import LoginForm from "../components/LoginForm";
import HomePage from "../components/HomePage/HomePage";
import SettingsPage from "../components/SettingsPage/SettingsPage";
import EditProfilePage from "../components/EditProfilePage/EditProfilePage";
import EditBioPage from "../components/EditBioPage/EditBioPage";

export const routes = [
    { path: paths.signUp, element: SignUpForm },
    { path: paths.login, element: LoginForm },
    { path: paths.home, element: HomePage },
    { path: paths.settings, element: SettingsPage },
    { path: paths.editProfile, element: EditProfilePage },
    { path: paths.editBio, element: EditBioPage }
];