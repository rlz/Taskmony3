import yes from "../../images/checkbox-yes.svg";
import no from "../../images/checkbox-no.svg";
import yesRadio from "../../images/radio-yes.svg";
import noRadio from "../../images/radio-no.svg";

type FilterProps = {
  label: string;
  checked?: boolean;
  radio?: boolean;
};

export const FilterItem = ({
  label,
  checked,
  radio,
  onChange
}: FilterProps) => {
  return (
    <div className={"gap-4 flex m-4"}>
      <img
        className={"cursor-pointer"}
        src={radio ? (checked ? yesRadio : noRadio) : checked ? yes : no}
        onClick={onChange}
      ></img>
      <span className={"font-semibold text-sm"}>{label}</span>
    </div>
  );
};
