type InputPropsT = {
  label: string;
  type: string;
  onChange: Function;
  value: string;
};

export const Input = ({ label, type, onChange, value }: InputPropsT) => {
  return (
    <>
      <input
        type={type}
        placeholder={label}
        onChange={(e) => onChange(e.target.value)}
        value={value}
        id={label}
        className="border w-full border-gray-300 rounded pl-2 pr-2 p-2 mt-2 mb-2"
      />
    </>
  );
};
