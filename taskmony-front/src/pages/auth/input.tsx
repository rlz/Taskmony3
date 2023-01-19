type InputPropsT = {
    label:string,
    type: string
  };

export const Input = ({ label, type } : InputPropsT) => {
    return (
      <>
      <input
        type={type}
        placeholder={label}
        className="border w-full border-gray-300 rounded mt-2 pl-1 pr-2"
      />
      </>
    );
  };