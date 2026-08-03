interface ButtonProps {
    title: string;
    style: string;
    onClick?: () => void;
}

const Button = ({ title, style, onClick } : ButtonProps) => {
    return (
        <button type="button" className={style} onClick={onClick}>
            {title}
        </button>
    )
}
export default Button;