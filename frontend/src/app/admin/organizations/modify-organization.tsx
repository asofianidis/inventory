"use client";

import { OrganizationBody, Organizations } from "@/types/organizations";
import {
  Sheet,
  SheetContent,
  SheetDescription,
  SheetHeader,
  SheetTitle,
  SheetTrigger,
} from "@/components/ui/sheet";
import { Button } from "@/components/ui/button";
import { useState } from "react";
import { Label } from "@/components/ui/label";
import { Input } from "@/components/ui/input";
import { useToast } from "@/components/ui/use-toast";
import { useRouter } from "next/navigation";

type props = {
  organization?: Organizations;
};

export default function ModifyOrganization({ organization }: props) {
  const [isOpen, setIsOpen] = useState(false);
  const [name, setName] = useState(organization?.name ?? "");

  const { toast } = useToast();
  const router = useRouter();

  const handleUpdate = async () => {
    try {
      const body: OrganizationBody = {
        org_id: organization?.org_id,
        org_name: name,
      };

      const res = await fetch(
        `${process.env.NEXT_PUBLIC_API_URL}/api/Organizations/UpdateOrganization`,
        {
          method: "POST",
          body: JSON.stringify(body),
          headers: { "Content-Type": "application/json" },
        }
      );

      if (!res.ok) {
        throw new Error(
          `Failed to post organizations body, status code ${res.status}`
        );
      }

      const data = await res.json();

      if (!isNaN(data)) {
        router.refresh();
        toast({ description: "Updated Organization" });
        setIsOpen(false);
      }
    } catch (e: any) {
      console.log(e);
      toast({ description: e.message, variant: "destructive" });
    }
  };
  return (
    <Sheet open={isOpen} onOpenChange={setIsOpen}>
      <SheetTrigger asChild>
        <Button>{organization != undefined ? "Edit" : "New"}</Button>
      </SheetTrigger>
      <SheetContent>
        <SheetHeader>
          <SheetTitle>
            {organization != undefined
              ? `Edit Organization: ${organization.name}`
              : "Create New Organization"}
          </SheetTitle>
        </SheetHeader>
        <div className="flex flex-1 flex-col gap-5">
          <div className="flex flex-col gap-2">
            <Label>Organization Name</Label>
            <Input
              value={name}
              onChange={(e) => {
                setName(e.target.value);
              }}
              placeholder="Organization Name"
            />
          </div>

          <div className="flex gap-2 items-center justify-end">
            <Button onClick={handleUpdate}>Save</Button>
            {organization != undefined ? (
              <>
                <Button variant={"destructive"}>Delete</Button>
              </>
            ) : null}
          </div>
        </div>
      </SheetContent>
    </Sheet>
  );
}
