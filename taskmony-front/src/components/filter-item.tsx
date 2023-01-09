import yes from "../images/checkbox-yes.svg";
import no from "../images/checkbox-yes.svg";

type FilterProps = {
  label: string;
  checked: boolean;
};

export const FilterItem = ({ label, checked }: FilterProps) => {
  return (
    <div className={"gap-4 flex m-4"}>
      <img src={checked? yes : no}></img>
      <span className={"font-semibold text-sm"}>{label}</span>
    </div>
  );
};
