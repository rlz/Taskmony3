import { FC } from "react";
import { Link } from "react-router-dom";

type MenuItemProps = {
    to: string, name: string, icon: string, isActive: boolean
}

export const MenuItem = ({to, name, icon, isActive}: MenuItemProps) => {
    return (
        <div className={"gap-4 flex m-4"}>
              <img src={icon}></img>
              <Link to={to}><p className={`${isActive?"underline":""} font-semibold text-sm`}>{name}</p></Link>
        </div>
    );  
  }
  
  export default MenuItem;