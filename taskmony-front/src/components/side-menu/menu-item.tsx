import { FC } from "react";
import { Link, NavLink } from "react-router-dom";

type MenuItemProps = {
    to: string, name: string, icon: string, isActive: boolean
}

export const MenuItem = ({to, name, icon, isActive}: MenuItemProps) => {
    const activeStyle = "underline underline-offset-4";
    return (
        <div className={"gap-4 flex m-4"}>
              <img src={icon}></img>
              <NavLink to={to} className={({ isActive }) => (isActive ? activeStyle : undefined)}><p className={`${isActive?"underline":""} font-semibold text-sm`}>{name}</p></NavLink>
        </div>
    );  
  }
  
  export default MenuItem;