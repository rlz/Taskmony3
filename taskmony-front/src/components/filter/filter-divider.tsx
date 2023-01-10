  import arrowUp from "../../images/arrow-up.svg";
import arrowDown from "../../images/arrow-down.svg";


type FilterProps = {
    setIsOpen: Function,
    isOpen: boolean,
    title: string
  };
  export const FilterDivider = ({ isOpen, setIsOpen, title }: FilterProps) => {
    return (
        <div className={"gap-4 flex m-4 justify-between"}>
        <p className={"font-bold text-gray-300 text-sm"}>{title}</p>
        <img
          src={isOpen ? arrowUp : arrowDown}
          onClick={()=>setIsOpen(!isOpen)}
        ></img>
      </div>
    );
  };