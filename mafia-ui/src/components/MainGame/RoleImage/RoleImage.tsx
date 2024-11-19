import React, { useEffect, useState } from 'react';
import './RoleImage.module.scss'; // Import your CSS file containing .role-image styles

interface RoleImageProps {
  roleName: string;
}

const RoleImage: React.FC<RoleImageProps> = ({ roleName }) => {
  const [imageSrc, setImageSrc] = useState<string | null>(null);

  useEffect(() => {
    const fetchRoleImage = async () => {
      try {
        const response = await fetch(`http://localhost:5141/api/gamecontrol/${roleName}`);
        if (response.ok) {
          const blob = await response.blob();
          const imageUrl = URL.createObjectURL(blob);
          setImageSrc(imageUrl);
        } else {
          console.error('Failed to fetch image:', response.statusText);
        }
      } catch (error) {
        console.error('Error fetching role image:', error);
      }
    };

    fetchRoleImage();
  }, [roleName]);

  console.log(`Fetching image for role: ${roleName}`);
  console.log(`Fetching from URL: http://localhost:5141/api/gamecontrol/${roleName}`);

  if (!imageSrc) {
    return <p>Loading image...</p>;
  }

  return <img
    className="role-image"
    src={imageSrc}
    alt={`${roleName} role`}
    style={{width: '200px', height: '200px'}} 
  />;
};

export default RoleImage;
